namespace Market.Application.Modules.Storage.Priviledges.Queries.List
{
    public sealed class ListPriviledgesQueryHandler(IAppDbContext ctx)
        : IRequestHandler<ListPriviledgesQuery, List<ListPriviledgesQueryDto>>
    {
        public async Task<List<ListPriviledgesQueryDto>> Handle(ListPriviledgesQuery request, CancellationToken cancellationToken)
        {
            return await ctx.Priviledges
                .AsNoTracking()
                .OrderBy(x => x.Code)
                .Select(x => new ListPriviledgesQueryDto
                {
                    Id = x.Id,
                    Name = x.Code,
                    Description = x.Description
                })
                .ToListAsync(cancellationToken);
        }
    }
}