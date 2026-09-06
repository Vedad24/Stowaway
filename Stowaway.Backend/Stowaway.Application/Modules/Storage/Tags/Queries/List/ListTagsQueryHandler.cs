using Stowaway.Application.Modules.Storage.Items.Shared;

namespace Stowaway.Application.Modules.Storage.Tags.Queries.List
{
    public class ListTagsQueryHandler(IAppDbContext ctx) : IRequestHandler<ListTagsQuery, List<SharedTagDto>>
    {
        public async Task<List<SharedTagDto>> Handle(ListTagsQuery request, CancellationToken cancellationToken)
        {
            return await ctx.Tags
                .OrderBy(t => t.Name)
                .Select(t => new SharedTagDto { Id = t.Id, Name = t.Name })
                .ToListAsync(cancellationToken);
        }
    }
}
