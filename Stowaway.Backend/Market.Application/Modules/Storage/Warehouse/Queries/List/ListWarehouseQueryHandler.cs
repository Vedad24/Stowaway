

namespace Stowaway.Application.Modules.Storage.Warehouse.Queries.List
{
    public sealed class ListWarehouseQueryHandler(IAppDbContext ctx)
        : IRequestHandler<ListWarehouseQuery, PageResult<ListWarehouseQueryDto>>
    {
        public async Task<PageResult<ListWarehouseQueryDto>> Handle(ListWarehouseQuery request, CancellationToken cancellationToken)
        {
            var query = ctx.Warehouses.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.Where(x => x.Name.Contains(request.Search));
            }

            if (!string.IsNullOrWhiteSpace(request.City))
            {
                query = query.Where(x => x.City.Contains(request.City));
            }

            if (!string.IsNullOrWhiteSpace(request.Address))
            {
                query = query.Where(x => x.Address.Contains(request.Address));
            }

            if (request.IsEnabled.HasValue)
            {
                query = query.Where(x => x.isEnabled == request.IsEnabled.Value);
            }

            if (request.MinCapacity.HasValue)
            {
                query = query.Where(x => x.Capacity >= request.MinCapacity.Value);
            }

            if (request.MaxCapacity.HasValue)
            {
                query = query.Where(x => x.Capacity <= request.MaxCapacity.Value);
            }

            var projectedQuery = query.Select(x => new ListWarehouseQueryDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                City = x.City,
                Address = x.Address,
                Capacity = x.Capacity,
                isEnabled = x.isEnabled
            });

            return await PageResult<ListWarehouseQueryDto>.FromQueryableAsync(projectedQuery, request.Paging, cancellationToken);
        }
    }
}
