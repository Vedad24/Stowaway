namespace Stowaway.Application.Modules.Dashboard.Queries.GetGlobalStats
{
    public sealed class GetGlobalStatsQueryHandler(IAppDbContext ctx)
        : IRequestHandler<GetGlobalStatsQuery, GetGlobalStatsQueryDto>
    {
        public async Task<GetGlobalStatsQueryDto> Handle(GetGlobalStatsQuery request, CancellationToken cancellationToken)
        {
            return new GetGlobalStatsQueryDto
            {
                TotalUsers = await ctx.Users.CountAsync(cancellationToken),
                TotalWarehouses = await ctx.Warehouses.CountAsync(cancellationToken),
                TotalContainers = await ctx.Containers.CountAsync(cancellationToken),
                TotalItems = await ctx.Item.CountAsync(cancellationToken),
                TotalSuppliers = await ctx.Suppliers.CountAsync(cancellationToken),
                TotalOrders = await ctx.Orders.CountAsync(cancellationToken)
            };
        }
    }
}
