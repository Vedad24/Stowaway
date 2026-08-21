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

            if (!string.IsNullOrWhiteSpace(request.Address))
            {
                var address = request.Address.Trim().ToLower();
                query = query.Where(x => x.Address.ToLower().Contains(address));
            }

            if (request.MinTotalDeliveries.HasValue)
            {
                query = query.Where(x => x.TotalDeliveries >= request.MinTotalDeliveries.Value);
            }

            if (request.MaxTotalDeliveries.HasValue)
            {
                query = query.Where(x => x.TotalDeliveries <= request.MaxTotalDeliveries.Value);
            }

            if (request.MinFailedDeliveries.HasValue)
            {
                query = query.Where(x => x.FailedDeliveries >= request.MinFailedDeliveries.Value);
            }

            if (request.MaxFailedDeliveries.HasValue)
            {
                query = query.Where(x => x.FailedDeliveries <= request.MaxFailedDeliveries.Value);
            }

            if (request.MinFailureRate.HasValue)
            {
                query = query.Where(x =>
                    x.TotalDeliveries > 0 &&
                    (x.FailedDeliveries * 100.0 / x.TotalDeliveries) >= request.MinFailureRate.Value);
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
