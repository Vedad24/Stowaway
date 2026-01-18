using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Application.Modules.Storage.Supplier.Queries.List
{
    public class ListSupplierQueryHandler(IAppDbContext ctx)
        : IRequestHandler<ListSupplierQuery, PageResult<ListSupplierQueryDto>>
    {
        public async Task<PageResult<ListSupplierQueryDto>> Handle(ListSupplierQuery request, CancellationToken cancellationToken)
        {
            var query = ctx.Suppliers.AsNoTracking();

            var searchTerm = request.Search?.Trim().ToLower() ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(x => x.Name.ToLower().Contains(searchTerm));
            }

            var projectedQuery = query.Select(x => new ListSupplierQueryDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Address = x.Address,
                FailedDeliveries = x.FailedDeliveries,
                TotalDeliveries = x.TotalDeliveries,
            });

            return await PageResult<ListSupplierQueryDto>.FromQueryableAsync(projectedQuery, request.Paging, cancellationToken);
        }
    }
}
