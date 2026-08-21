using Stowaway.Application.Modules.Storage.Container.Shared;
using Stowaway.Domain.Entities.Storage;

namespace Stowaway.Application.Modules.Storage.Items.Queries.List
{
    public class ListContainersQueryHandler(IAppDbContext ctx)
        : IRequestHandler<ListContainersQuery, List<ListContainersDto>>
    {
        public async Task<List<ListContainersDto>> Handle(ListContainersQuery request, CancellationToken cancellationToken)
        {
            var query = ctx.Containers.AsNoTracking();

            var searchTerm = request.Search?.Trim().ToLower() ?? string.Empty;

            if(!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(x => x.Name.ToLower().Contains(searchTerm));
            }

            if (request.WarehouseId.HasValue)
            {
                query = query.Where(x => x.WarehouseId == request.WarehouseId.Value);
            }

            query = request.ParentContainerId.HasValue
                ? query.Where(x => x.ParentContainerId == request.ParentContainerId.Value)
                : query.Where(x => x.ParentContainerId == null);

            if (request.ContainerTypeId.HasValue)
            {
                query = query.Where(x => x.ContainerTypeId == request.ContainerTypeId.Value);
            }

            if (request.StatusId.HasValue)
            {
                query = query.Where(x => ctx.ContainerStatusHistories
                    .Where(h => h.ContainerId == x.Id)
                    .OrderByDescending(h => h.Date)
                    .Select(h => h.StatusId)
                    .FirstOrDefault() == request.StatusId.Value);
            }

            var projectedQuery = query.Select(x => new ListContainersDto
            {
                Id = x.Id,
                Name = x.Name,
                WarehouseId = x.WarehouseId,
                WarehouseName = x.Warehouse!.Name,
                ParentContainerId = x.ParentContainerId,
                HasChildren = ctx.Containers.Any(child => child.ParentContainerId == x.Id),
                CanvasX = x.CanvasX,
                CanvasY = x.CanvasY,
                MaxItems = x.ContainerType!.MaxItems,
                MaxContainers = x.ContainerType!.MaxContainers,
                ContainerCountUsed = ctx.Containers.Count(child => child.ParentContainerId == x.Id),
                CurrentStatus = ctx.ContainerStatusHistories
                    .Where(h => h.ContainerId == x.Id)
                    .OrderByDescending(h => h.Date)
                    .Select(h => new SharedContainerStatusDto { Id = h.Status.Id, Name = h.Status.Description })
                    .FirstOrDefault(),
            });

            var results = await projectedQuery.ToListAsync(cancellationToken);

            // Recursive (self + nested sub-containers) and the type's human-readable size
            // label — not translatable into the SQL projection above, so filled in per row
            // once the base rows are materialized.
            foreach (var result in results)
            {
                result.ItemQuantityUsed = await ContainerCapacityHelper.GetRecursiveItemQuantity(ctx, result.Id, cancellationToken);
                result.ContainerTypeName = ContainerTypeEntity.DescribeSize(result.MaxItems, result.MaxContainers);
            }

            return results;
        }
    }
}
