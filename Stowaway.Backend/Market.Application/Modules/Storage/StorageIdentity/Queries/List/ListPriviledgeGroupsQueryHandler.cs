namespace Stowaway.Application.Modules.Storage.StorageIdentity.Queries.List
{
    public sealed class ListPriviledgeGroupsQueryHandler(IAppDbContext ctx)
        : IRequestHandler<ListPriviledgeGroupsQuery, List<ListPriviledgeGroupQueryDto>>
    {
        public async Task<List<ListPriviledgeGroupQueryDto>> Handle(ListPriviledgeGroupsQuery request, CancellationToken cancellationToken)
        {
            var query = ctx.PriviledgeGroups
                .AsNoTracking()
                .Include(x => x.Priviledges)
                .AsQueryable();

            if (request.WarehouseId.HasValue)
            {
                query = query.Where(x => x.WarehouseId == request.WarehouseId.Value);
            }

            return await query
                .OrderBy(x => x.Name)
                .Select(x => new ListPriviledgeGroupQueryDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    WarehouseId = x.WarehouseId,
                    PriviledgeIds = x.Priviledges != null
                        ? x.Priviledges.Select(p => p.PriviledgeId).ToList()
                        : new List<int>()
                })
                .ToListAsync(cancellationToken);
        }
    }
}
