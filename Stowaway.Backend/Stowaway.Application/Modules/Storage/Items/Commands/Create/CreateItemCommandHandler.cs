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

            var validTagIds = request.TagIds.Count > 0
                ? await ctx.Tags.Where(t => request.TagIds.Contains(t.Id)).Select(t => t.Id).ToListAsync(cancellationToken)
                : [];

            var item = new ItemEntity
            {
                Name = normalizedName,
                Description = request.Description,
                ByteImage = request.Images.Count > 0 ? Convert.FromBase64String(request.Images[0]) : request.ByteImage,
                Quantity = request.Quantity,
                SupplierId = request.SupplierId,
                ContainerId = request.ContainerId,
                Images = request.Images.Select((image, i) => new ItemImageEntity
                {
                    ByteImage = Convert.FromBase64String(image),
                    SortOrder = i,
                }).ToList(),
            };

            ctx.Item.Add(item);

            foreach (var tagId in validTagIds)
            {
                ctx.ItemTags.Add(new Item_TagEntity { Item = item, TagId = tagId });
            }

            await ctx.SaveChangesAsync(cancellationToken);

            return item.Id;
        }
    }
}
