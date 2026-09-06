using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stowaway.Domain.Entities.Storage;

namespace Stowaway.Application.Modules.Storage.Items.Commands.SetFavourite
{
    public class SetItemFavouriteCommandHandler(IAppDbContext ctx, IAppCurrentUser appCurrentUser)
        : IRequestHandler<SetItemFavouriteCommand, Unit>
    {
        public async Task<Unit> Handle(SetItemFavouriteCommand request, CancellationToken cancellationToken)
        {
            var itemExists = await ctx.Item.AnyAsync(x => x.Id == request.Id, cancellationToken);

            if (!itemExists)
            {
                throw new StowawayNotFoundException($"Item with id: {request.Id} not found");
            }

            if (appCurrentUser.UserId is null)
            {
                throw new StowawayUnauthorizedException("Current user could not be resolved.");
            }

            var existing = await ctx.ItemFavourites.FirstOrDefaultAsync(
                f => f.ItemId == request.Id && f.UserId == appCurrentUser.UserId, cancellationToken);

            if (request.IsFavourite && existing is null)
            {
                ctx.ItemFavourites.Add(new Item_UserFavouriteEntity
                {
                    ItemId = request.Id,
                    UserId = appCurrentUser.UserId.Value,
                });
            }
            else if (!request.IsFavourite && existing is not null)
            {
                ctx.ItemFavourites.Remove(existing);
            }

            await ctx.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
