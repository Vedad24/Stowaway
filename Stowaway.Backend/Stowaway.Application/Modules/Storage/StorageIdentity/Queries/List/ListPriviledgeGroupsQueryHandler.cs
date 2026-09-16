namespace Stowaway.Application.Modules.Storage.StorageIdentity.Queries.List
{
    public sealed class ListPriviledgeGroupsQueryHandler(IAppDbContext ctx, IAppCurrentUser appCurrentUser)
        : IRequestHandler<ListPriviledgeGroupsQuery, PageResult<ListPriviledgeGroupQueryDto>>
    {
        public async Task<PageResult<ListPriviledgeGroupQueryDto>> Handle(ListPriviledgeGroupsQuery request, CancellationToken cancellationToken)
        {
            var query = ctx.PriviledgeGroups
                .AsNoTracking()
                .Include(x => x.Priviledges)
                .AsQueryable();

            if (!appCurrentUser.IsAdmin)
            {
                query = query.Where(x => ctx.WarehouseUsers.Any(wu => wu.WarehouseId == x.WarehouseId && wu.UserId == appCurrentUser.UserId));
            }

            if (request.WarehouseId.HasValue)
            {
                query = query.Where(x => x.WarehouseId == request.WarehouseId.Value);
            }

            var projectedQuery = query
                .OrderBy(x => x.Name)
                .Select(x => new ListPriviledgeGroupQueryDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    WarehouseId = x.WarehouseId,
                    PriviledgeIds = x.Priviledges != null
                        ? x.Priviledges.Select(p => p.PriviledgeId).ToList()
                        : new List<int>()
                });

            return await PageResult<ListPriviledgeGroupQueryDto>.FromQueryableAsync(projectedQuery, request.Paging, cancellationToken);
        }
    }
}
