using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stowaway.Application.Modules.Storage.Container.Shared;

namespace Stowaway.Application.Modules.Storage.Items.Commands.Update
{
    public class UpdateItemCommandHandler(IAppDbContext ctx) : IRequestHandler<UpdateItemCommand, Unit>
    {
        public async Task<Unit> Handle(UpdateItemCommand request, CancellationToken cancellationToken)
        {
            var item = await ctx.Item.Where(x => x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);

            if (item is null)
            {
                throw new Exception($"Item with id: {request.Id} not found");
            }

            var supplier = await ctx.Suppliers.Where(x=> x.Id == request.SupplierId).FirstOrDefaultAsync( cancellationToken);

            if (supplier is null)
            {
                throw new Exception($"Supplier with id: {request.Id} not found");
            }

            var container = await ctx.Containers.FirstOrDefaultAsync(x => x.Id.Equals(request.ContainerId), cancellationToken);
            if (container == null)
            {
                throw new Exception("Container does not exist");
            }

            if(request.Quantity < 1)
            {
                throw new Exception("Quantity cant be less than 1");
            }

            await ContainerCapacityHelper.EnsureItemFits(ctx, container.Id, request.Quantity, cancellationToken, excludeItemIds: new[] { item.Id });

            item.Name = request.Name;
            item.Description = request.Description;
            item.ByteImage = request.ByteImage;
            item.Quantity = request.Quantity;
            item.SupplierId = request.SupplierId;
            item.ContainerId = request.ContainerId;

            await ctx.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
