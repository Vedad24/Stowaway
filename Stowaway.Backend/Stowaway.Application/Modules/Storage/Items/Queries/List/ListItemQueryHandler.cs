using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Stowaway.Application.Abstractions;
using Stowaway.Application.Modules.Storage.Container.Shared;
using Stowaway.Application.Modules.Storage.Items.Shared;
using Stowaway.Application.Modules.Storage.Supplier.Shared;

namespace Stowaway.Application.Modules.Storage.Items.Queries.List
{
    public class ListItemQueryHandler(IAppDbContext ctx, IAppCurrentUser appCurrentUser)
        : IRequestHandler<ListItemQuery, PageResult<ListItemQueryDto>>
    {
        public async Task<PageResult<ListItemQueryDto>> Handle(ListItemQuery request, CancellationToken cancellationToken)
        {
            var accessibleWarehouseIds = appCurrentUser.IsAdmin
                ? ctx.Warehouses.Select(w => w.Id)
                : ctx.WarehouseUsers
                    .Where(wu => wu.UserId == appCurrentUser.UserId
                        && wu.PriviledgeGroup.Priviledges.Any(p => p.Priviledge.Code == Stowaway.Shared.Constants.Priviledges.ItemRead))
                    .Select(wu => wu.WarehouseId);

            var query = ctx.Item.AsNoTracking()
                .Where(x => accessibleWarehouseIds.Contains(x.Container.WarehouseId));

            var searchTerm = request.Search?.Trim().ToLower() ?? string.Empty;

            if(!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(x => x.Name.ToLower().Contains(searchTerm));
            }

            if (request.ContainerId.HasValue)
            {
                query = query.Where(x => x.ContainerId == request.ContainerId.Value);
            }

            if (request.SupplierId.HasValue)
            {
                query = query.Where(x => x.SupplierId == request.SupplierId.Value);
            }

            if (request.TagIds is { Count: > 0 })
            {
                query = query.Where(x => ctx.ItemTags.Any(it => it.ItemId == x.Id && request.TagIds.Contains(it.TagId)));
            }

            if (request.MinQuantity.HasValue)
            {
                query = query.Where(x => x.Quantity >= request.MinQuantity.Value);
            }

            if (request.MaxQuantity.HasValue)
            {
                query = query.Where(x => x.Quantity <= request.MaxQuantity.Value);
            }

            if (request.FavouritesOnly == true)
            {
                query = query.Where(x => ctx.ItemFavourites.Any(f => f.ItemId == x.Id && f.UserId == appCurrentUser.UserId));
            }

            var projectedQuery = query.Select(x => new ListItemQueryDto
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
                CanvasX = x.CanvasX,
                CanvasY = x.CanvasY,
                IsFavourite = ctx.ItemFavourites.Any(f => f.ItemId == x.Id && f.UserId == appCurrentUser.UserId),
            });

            return await PageResult<ListItemQueryDto>.FromQueryableAsync(projectedQuery, request.Paging, cancellationToken);
        }
    }
}
