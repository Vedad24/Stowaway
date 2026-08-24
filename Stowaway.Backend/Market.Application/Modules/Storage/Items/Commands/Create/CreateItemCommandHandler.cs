using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stowaway.Application.Modules.Storage.Container.Shared;
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
                throw new StowawayBusinessRuleException("item.name-required", "Item name is required");
            }

            //bool exists = await ctx.Item.AnyAsync(x => x.Name == normalizedName, cancellationToken);

            //if (exists) 
            //{
            //    throw new Exception($"Item with {normalizedName} already exists");            
            //}

            var supplier = await ctx.Suppliers.Where(x => x.Id == request.SupplierId).FirstOrDefaultAsync(cancellationToken);

            if (supplier == null)
            {
                throw new StowawayBusinessRuleException("item.invalid-supplier", "Supplier is not valid");
            }

            var container = await ctx.Containers.FirstOrDefaultAsync(x => x.Id.Equals(request.ContainerId), cancellationToken);
            if (container == null)
            {
                throw new StowawayNotFoundException("Container does not exist");
            }

            if (request.Quantity < 1)
            {
                throw new StowawayBusinessRuleException("item.invalid-quantity", "Quantity cant be less than 1");
            }

            await ContainerCapacityHelper.EnsureItemFits(ctx, container.Id, request.Quantity, cancellationToken);

            var item = new ItemEntity
            {
                Name = normalizedName,
                Description = request.Description,
                ByteImage = request.Images.Count > 0 ? Convert.FromBase64String(request.Images[0]) : request.ByteImage,
                Quantity = request.Quantity,
                SupplierId = request.SupplierId,
                ContainerId = request.ContainerId,
            };

            ctx.Item.Add(item);
            await ctx.SaveChangesAsync(cancellationToken);

            if (request.TagIds.Count > 0)
            {
                var validTagIds = await ctx.Tags
                    .Where(t => request.TagIds.Contains(t.Id))
                    .Select(t => t.Id)
                    .ToListAsync(cancellationToken);

                foreach (var tagId in validTagIds)
                {
                    ctx.ItemTags.Add(new Item_TagEntity { ItemId = item.Id, TagId = tagId });
                }

                await ctx.SaveChangesAsync(cancellationToken);
            }

            if (request.Images.Count > 0)
            {
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
            }

            return item.Id;
        }
    }
}
