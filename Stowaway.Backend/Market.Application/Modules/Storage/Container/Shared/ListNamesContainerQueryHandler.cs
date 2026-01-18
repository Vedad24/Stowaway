using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Stowaway.Application.Modules.Storage.Container.Shared;
using Stowaway.Application.Modules.Storage.Supplier.Shared;

namespace Stowaway.Application.Modules.Storage.Items.Queries.List
{
    public class ListNamesContainerQueryHandler(IAppDbContext ctx)
        : IRequestHandler<ListNamesContainerQuery, PageResult<SharedContainerDto>>
    {
        public async Task<PageResult<SharedContainerDto>> Handle(ListNamesContainerQuery request, CancellationToken cancellationToken)
        {
            var query = ctx.Containers.AsNoTracking();

            var searchTerm = request.Search?.Trim().ToLower() ?? string.Empty;

            if(!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(x => x.Name.ToLower().Contains(searchTerm));
            }

            var projectedQuery = query.Select(x => new SharedContainerDto
            {
                Id = x.Id,
                Name = x.Name,   
            });

            return await PageResult<SharedContainerDto>.FromQueryableAsync(projectedQuery, request.Paging, cancellationToken);
        }
    }
}
