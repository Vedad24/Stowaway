using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Application.Modules.Storage.Supplier.Queries.GetById
{
    public class GetSupplierByIdQueryHandler(IAppDbContext ctx)
        : IRequestHandler<GetSupplierByIdQuery, GetSupplierByIdQueryDto>
    {
        public async Task<GetSupplierByIdQueryDto> Handle(GetSupplierByIdQuery request, CancellationToken cancellationToken)
        {
            var supplier = await ctx.Suppliers.
                Where(x => x.Id == request.Id)
                .Select(x => new GetSupplierByIdQueryDto
                {
                    Id = request.Id,
                    Name = x.Name,
                    Description = x.Description,
                    Address = x.Address,
                    TotalDeliveries = x.TotalDeliveries,
                    FailedDeliveries = x.FailedDeliveries,
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (supplier is null) 
            {
                throw new Exception($"Supplier with id: {request.Id} is not found");   
            }

            return supplier;
        }
    }
}
