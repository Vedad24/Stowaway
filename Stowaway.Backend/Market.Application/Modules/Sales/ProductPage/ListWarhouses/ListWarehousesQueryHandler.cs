namespace Stowaway.Application.Modules.Sales.ProductPage.ListWarhouses
{
    public sealed class ListWarehousesQueryHandler(IAppDbContext ctx, IAppCurrentUser appCurrentUser)
        : IRequestHandler<ListWarehousesQuery, PageResult<ListWarehousesQueryDto>>
    {
        public async Task<PageResult<ListWarehousesQueryDto>> Handle(ListWarehousesQuery request, CancellationToken cancellationToken)
        {
            var query = ctx.WarehouseUsers.Include(wu => wu.Warehouse).Where(wu => wu.UserId == appCurrentUser.UserId).AsNoTracking();

            var projectedQuery = query.Select(x => new ListWarehousesQueryDto
            {
                Id = x.Warehouse.Id,
                Name = x.Warehouse.Name
            });

            return await PageResult<ListWarehousesQueryDto>.FromQueryableAsync(projectedQuery, request.Paging, cancellationToken);
        }
    }
}
