using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stowaway.Application.Modules.Storage.Container.Shared;
using Stowaway.Domain.Entities.Storage;

namespace Stowaway.Application.Modules.Storage.Items.Commands.Update
{
    public class UpdateItemCommandHandler(IAppDbContext ctx) : IRequestHandler<UpdateItemCommand, Unit>
    {
        public async Task<Unit> Handle(UpdateItemCommand request, CancellationToken cancellationToken)
        {
            var item = await ctx.Item.Where(x => x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);

            if (item is null)
            {
                throw new StowawayNotFoundException($"Item with id: {request.Id} not found");
            }

            var supplier = await ctx.Suppliers.Where(x=> x.Id == request.SupplierId).FirstOrDefaultAsync( cancellationToken);

            if (supplier is null)
            {
                throw new StowawayNotFoundException($"Supplier with id: {request.Id} not found");
            }

            var container = await ctx.Containers.FirstOrDefaultAsync(x => x.Id.Equals(request.ContainerId), cancellationToken);
            if (container == null)
            {
                throw new StowawayNotFoundException("Container does not exist");
            }

            var currentWarehouseId = await ctx.Containers
                .Where(x => x.Id == item.ContainerId)
                .Select(x => x.WarehouseId)
                .FirstOrDefaultAsync(cancellationToken);

            if (container.WarehouseId != currentWarehouseId)
            {
                throw new StowawayBusinessRuleException("item.cross-warehouse", "Cannot move an item into a different warehouse");
            }

            if(request.Quantity < 1)
            {
                throw new StowawayBusinessRuleException("item.invalid-quantity", "Quantity cant be less than 1");
            }

            await ContainerCapacityHelper.EnsureItemFits(ctx, container.Id, request.Quantity, cancellationToken, excludeItemIds: new[] { item.Id });

            item.Name = request.Name;
            item.Description = request.Description;
            item.ByteImage = request.Images.Count > 0 ? Convert.FromBase64String(request.Images[0]) : request.ByteImage;
            item.Quantity = request.Quantity;
            item.SupplierId = request.SupplierId;
            item.ContainerId = request.ContainerId;

            var validTagIds = await ctx.Tags
                .Where(t => request.TagIds.Contains(t.Id))
                .Select(t => t.Id)
                .ToListAsync(cancellationToken);

            var existingTags = await ctx.ItemTags
                .Where(it => it.ItemId == item.Id)
                .ToListAsync(cancellationToken);

            var tagsToRemove = existingTags.Where(it => !validTagIds.Contains(it.TagId));
            ctx.ItemTags.RemoveRange(tagsToRemove);

            var existingTagIds = existingTags.Select(it => it.TagId).ToHashSet();
            foreach (var tagId in validTagIds.Where(id => !existingTagIds.Contains(id)))
            {
                ctx.ItemTags.Add(new Item_TagEntity { ItemId = item.Id, TagId = tagId });
            }

            var existingImages = await ctx.ItemImages
                .Where(i => i.ItemId == item.Id)
                .ToListAsync(cancellationToken);
            ctx.ItemImages.RemoveRange(existingImages);

            for (int i = 0; i < request.Images.Count; i++)
            {
                ctx.ItemImages.Add(new ItemImageEntity
                {
                    ItemId = item.Id,
                    ByteImage = Convert.FromBase64String(request.Images[i]),
                    SortOrder = i,
                });
            }

            await ctx.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
