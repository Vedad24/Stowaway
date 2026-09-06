using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Stowaway.Application.Modules.Storage.Container.Shared;
using Stowaway.Application.Modules.Storage.Items.Shared;
using Stowaway.Application.Modules.Storage.Supplier.Shared;

namespace Stowaway.Application.Modules.Storage.Items.Queries.GetById
{
    public class GetItemByIdQueryHandler(IAppDbContext ctx, IAppCurrentUser appCurrentUser)
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
                    Id = x.Container.Id,
                    Name = x.Container.Name
                },
                Tags = ctx.ItemTags
                    .Where(it => it.ItemId == x.Id)
                    .Select(it => new SharedTagDto { Id = it.Tag.Id, Name = it.Tag.Name })
                    .ToList(),
                Images = ctx.ItemImages
                    .Where(i => i.ItemId == x.Id)
                    .OrderBy(i => i.SortOrder)
                    .Select(i => new SharedItemImageDto { Id = i.Id, ByteImage = i.ByteImage, SortOrder = i.SortOrder })
                    .ToList(),
                IsFavourite = ctx.ItemFavourites.Any(f => f.ItemId == x.Id && f.UserId == appCurrentUser.UserId),
            }).FirstOrDefaultAsync(cancellationToken);

            if (item == null)
            {
                throw new StowawayNotFoundException($"Item not found with id: {request.Id}");
            }

            return item;
        }
    }
}
