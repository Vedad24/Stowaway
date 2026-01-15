using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stowaway.Application.Modules.Storage.Items.Queries.GetById;
using Stowaway.Domain.Entities.Storage;

namespace Stowaway.Application.Modules.Storage.Items.Commands.Create
{
    public class CreateItemCommandHandler(IAppDbContext ctx)
        : IRequestHandler<CreateItemCommand, int>
    {
        public async Task<int> Handle(CreateItemCommand request, CancellationToken cancellationToken)
        {
            var normalizedName = request.Name?.Trim();

            if (string.IsNullOrWhiteSpace(normalizedName))
            {
                throw new Exception("Item name is required");
            }

            //bool exists = await ctx.Item.AnyAsync(x => x.Name == normalizedName, cancellationToken);

            //if (exists) 
            //{
            //    throw new Exception($"Item with {normalizedName} already exists");            
            //}


            var supplier = await ctx.Suppliers.Where(x => x.Id == request.SupplierId).FirstOrDefaultAsync(cancellationToken);

            if (supplier == null)
            {
                throw new Exception("Supplier is not valid");
            }

            var container = await ctx.Containers.FirstOrDefaultAsync(x => x.Id.Equals(request.ContainerId), cancellationToken);
            if (container == null) 
            {
                throw new Exception("Container does not exist");
            }

            if (request.Quantity < 1)
            {
                throw new Exception("Quantity cant be less than 1");
            }

            var item = new ItemEntity
            {
                Name = normalizedName,
                Description = request.Description,
                ByteImage = request.ByteImage,
                Quantity = request.Quantity,
                SupplierId = request.SupplierId,
                ContainerId = request.ContainerId,
            };

            ctx.Item.Add(item);
            await ctx.SaveChangesAsync(cancellationToken);

            return item.Id;
        }
    }
}
