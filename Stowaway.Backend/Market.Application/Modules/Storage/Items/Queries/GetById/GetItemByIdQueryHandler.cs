using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Stowaway.Application.Modules.Storage.Container.Shared;
using Stowaway.Application.Modules.Storage.Supplier.Shared;

namespace Stowaway.Application.Modules.Storage.Items.Queries.GetById
{
    public class GetItemByIdQueryHandler(IAppDbContext ctx)
        : IRequestHandler<GetItemByIdQuery, GetItemByIdQueryDto>
    {
        public async Task<GetItemByIdQueryDto> Handle(GetItemByIdQuery request, CancellationToken cancellationToken)
        {
            var q = ctx.Item.Where(x => x.Id == request.Id);

            var item = await q.Select(x => new GetItemByIdQueryDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                ByteImage = x.ByteImage,
                Quantity = x.Quantity,
                Supplier = new SharedSupplierDto
                {
                    Id = x.Supplier.Id,
                    Name = x.Supplier.Name,
                    Description = x.Supplier.Description,
                    Address = x.Supplier.Address,
                    FailedDeliveries = x.Supplier.FailedDeliveries,
                    TotalDeliveries = x.Supplier.TotalDeliveries,
                },
                Container = new ListContainersDto 
                {    
                    Id = x.Id,
                    Name = x.Container.Name
                }
            }).FirstOrDefaultAsync(cancellationToken);

            if (item == null)
            {
                throw new Exception($"Item not found with id: {request.Id}");
            }

            return item;
        }
    }
}
