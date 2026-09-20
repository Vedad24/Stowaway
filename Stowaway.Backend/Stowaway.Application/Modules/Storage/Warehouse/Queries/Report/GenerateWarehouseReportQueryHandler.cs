using Stowaway.Application.Abstractions.Reporting;
using Stowaway.Application.Modules.Storage.Container.Shared;
using Stowaway.Domain.Entities.Storage;

namespace Stowaway.Application.Modules.Storage.Warehouse.Queries.Report
{
    public sealed class GenerateWarehouseReportQueryHandler(
        IAppDbContext ctx,
        IAppCurrentUser appCurrentUser,
        IPdfReportGenerator pdfGenerator)
        : IRequestHandler<GenerateWarehouseReportQuery, GenerateWarehouseReportResult>
    {
        public async Task<GenerateWarehouseReportResult> Handle(GenerateWarehouseReportQuery request, CancellationToken cancellationToken)
        {
            var warehouseQuery = ctx.Warehouses.Where(x => x.Id == request.WarehouseId);

            if (!appCurrentUser.IsAdmin)
            {
                warehouseQuery = warehouseQuery.Where(x => ctx.WarehouseUsers.Any(wu => wu.WarehouseId == x.Id && wu.UserId == appCurrentUser.UserId));
            }

            var warehouseName = await warehouseQuery.Select(x => x.Name).FirstOrDefaultAsync(cancellationToken);

            if (warehouseName is null)
            {
                throw new StowawayNotFoundException($"Warehouse with ID : {request.WarehouseId} not found");
            }

            var reportType = string.IsNullOrWhiteSpace(request.ReportType) ? "both" : request.ReportType.ToLowerInvariant();
            var showContainers = reportType != "items";
            var showItems = reportType != "containers";

            var containers = await ctx.Containers.AsNoTracking()
                .Where(c => c.WarehouseId == request.WarehouseId)
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.ParentContainerId,
                    MaxItems = c.ContainerType!.MaxItems,
                    MaxContainers = c.ContainerType!.MaxContainers,
                    ContainerCountUsed = ctx.Containers.Count(x => x.ParentContainerId == c.Id),
                    CurrentStatusName = ctx.ContainerStatusHistories
                        .Where(h => h.ContainerId == c.Id)
                        .OrderByDescending(h => h.Date)
                        .Select(h => h.Status.Description)
                        .FirstOrDefault(),
                })
                .ToListAsync(cancellationToken);

            var itemsUsedByContainer = await ContainerCapacityHelper.GetRecursiveItemQuantities(
                ctx, containers.Select(c => c.Id).ToList(), cancellationToken);

            // Containers are grouped by parent id; root containers (ParentContainerId == null) are
            // bucketed under RootKey instead, since Nullable<int> can't be used as a dictionary key
            // (Dictionary<TKey,_> requires TKey : notnull, and int? doesn't satisfy that constraint).
            const int RootKey = 0;

            var childrenByParent = new Dictionary<int, List<int>>();
            foreach (var group in containers.GroupBy(c => c.ParentContainerId ?? RootKey))
            {
                childrenByParent[group.Key] = group.OrderBy(c => c.Name, StringComparer.OrdinalIgnoreCase).Select(c => c.Id).ToList();
            }

            var containersById = containers.ToDictionary(c => c.Id);

            var containerRows = new List<WarehouseReportContainerRow>();
            var pathByContainerId = new Dictionary<int, string>();
            var orderByContainerId = new Dictionary<int, int>();

            void Visit(int parentKey, string ancestorPath)
            {
                if (!childrenByParent.TryGetValue(parentKey, out var childIds))
                {
                    return;
                }

                foreach (var childId in childIds)
                {
                    var c = containersById[childId];
                    var fullPath = ancestorPath.Length == 0 ? c.Name : $"{ancestorPath} / {c.Name}";
                    pathByContainerId[c.Id] = fullPath;
                    orderByContainerId[c.Id] = orderByContainerId.Count;

                    containerRows.Add(new WarehouseReportContainerRow(
                        c.Name,
                        ancestorPath.Length == 0 ? "—" : ancestorPath,
                        ContainerTypeEntity.DescribeSize(c.MaxItems, c.MaxContainers),
                        itemsUsedByContainer.GetValueOrDefault(c.Id),
                        c.MaxItems,
                        c.ContainerCountUsed,
                        c.MaxContainers,
                        c.CurrentStatusName ?? "—"));

                    Visit(c.Id, fullPath);
                }
            }

            Visit(RootKey, string.Empty);

            var itemRows = new List<WarehouseReportItemRow>();

            if (showItems)
            {
                var containerIds = containers.Select(c => c.Id).ToList();

                var items = await ctx.Item.AsNoTracking()
                    .Where(i => containerIds.Contains(i.ContainerId))
                    .Select(i => new
                    {
                        i.Name,
                        i.ContainerId,
                        i.Quantity,
                        SupplierName = i.Supplier != null ? i.Supplier.Name : null,
                        Tags = ctx.ItemTags.Where(it => it.ItemId == i.Id).Select(it => it.Tag!.Name).ToList(),
                    })
                    .ToListAsync(cancellationToken);

                itemRows = items
                    .OrderBy(i => orderByContainerId.GetValueOrDefault(i.ContainerId))
                    .ThenBy(i => i.Name, StringComparer.OrdinalIgnoreCase)
                    .Select(i => new WarehouseReportItemRow(
                        i.Name,
                        pathByContainerId.GetValueOrDefault(i.ContainerId, "—"),
                        i.Quantity,
                        i.SupplierName ?? "—",
                        string.Join(", ", i.Tags)))
                    .ToList();
            }

            var reportTypeLabel = reportType switch
            {
                "containers" => "Containers report",
                "items" => "Items report",
                _ => "Containers and items report",
            };

            IReadOnlyList<string> containerColumns = request.ContainerColumns is { Count: > 0 }
                ? request.ContainerColumns
                : WarehouseReportColumns.AllContainerColumns;
            IReadOnlyList<string> itemColumns = request.ItemColumns is { Count: > 0 }
                ? request.ItemColumns
                : WarehouseReportColumns.AllItemColumns;

            var reportData = new WarehouseReportData(
                warehouseName,
                reportTypeLabel,
                DateTime.UtcNow,
                showContainers,
                showItems,
                containerRows,
                itemRows,
                containerColumns,
                itemColumns);

            var fileContent = pdfGenerator.GenerateWarehouseReport(reportData);
            var fileName = BuildFileName(warehouseName);

            return new GenerateWarehouseReportResult
            {
                FileContent = fileContent,
                FileName = fileName,
            };
        }

        private static string BuildFileName(string warehouseName)
        {
            var slug = new string(warehouseName
                .Trim()
                .ToLowerInvariant()
                .Select(ch => char.IsLetterOrDigit(ch) ? ch : '-')
                .ToArray());

            while (slug.Contains("--"))
            {
                slug = slug.Replace("--", "-");
            }

            slug = slug.Trim('-');

            if (string.IsNullOrEmpty(slug))
            {
                slug = "warehouse";
            }

            return $"{slug}-report-{DateTime.UtcNow:yyyyMMdd-HHmmss}.pdf";
        }
    }
}
