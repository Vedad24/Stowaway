using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stowaway.Application.Modules.Storage.Container.Shared;

namespace Stowaway.Application.Modules.Storage.Items.Commands.Move
{
    public class MoveItemCommandHandler(IAppDbContext ctx) : IRequestHandler<MoveItemCommand, Unit>
    {
        public async Task<Unit> Handle(MoveItemCommand request, CancellationToken cancellationToken)
        {
            var item = await ctx.Item.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (item is null)
            {
                throw new Exception($"Item with id: {request.Id} not found");
            }

            var container = await ctx.Containers.FirstOrDefaultAsync(x => x.Id == request.ContainerId, cancellationToken);
            if (container is null)
            {
                throw new Exception($"Container with id: {request.ContainerId} not found");
            }

            await ContainerCapacityHelper.EnsureItemFits(ctx, container.Id, item.Quantity, cancellationToken, excludeItemIds: new[] { item.Id });

            item.ContainerId = container.Id;
            item.CanvasX = null;
            item.CanvasY = null;

            await ctx.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
