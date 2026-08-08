using Stowaway.Application.Modules.Storage.Container.Shared;

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

            var projectedQuery = query.Select(x => new ListContainersDto
            {
                Id = x.Id,
                Name = x.Name,
                ContainerTypeId = x.ContainerTypeId,
                WarehouseId = x.WarehouseId,
                ParentContainerId = x.ParentContainerId,
                HasChildren = ctx.Containers.Any(child => child.ParentContainerId == x.Id),
                CanvasX = x.CanvasX,
                CanvasY = x.CanvasY,
                MaxItems = x.ContainerType!.MaxItems,
                MaxContainers = x.ContainerType!.MaxContainers,
                ContainerCountUsed = ctx.Containers.Count(child => child.ParentContainerId == x.Id),
            });

            var results = await projectedQuery.ToListAsync(cancellationToken);

            // Recursive (self + nested sub-containers) — not translatable into the SQL
            // projection above, so it's filled in per row once the base rows are materialized.
            foreach (var result in results)
            {
                result.ItemQuantityUsed = await ContainerCapacityHelper.GetRecursiveItemQuantity(ctx, result.Id, cancellationToken);
            }

            return results;
        }
    }
}
