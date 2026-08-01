using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Application.Modules.Storage.Container.Commands.Move
{
    public class MoveContainerCommandHandler(IAppDbContext ctx) : IRequestHandler<MoveContainerCommand, Unit>
    {
        public async Task<Unit> Handle(MoveContainerCommand request, CancellationToken cancellationToken)
        {
            var container = await ctx.Containers.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (container is null)
            {
                throw new Exception($"Container with id: {request.Id} not found");
            }

            if (request.ParentContainerId is null)
            {
                container.ParentContainerId = null;
                container.CanvasX = null;
                container.CanvasY = null;

                await ctx.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }

            if (request.Id == request.ParentContainerId)
            {
                throw new Exception("A container cannot be placed inside itself");
            }

            var target = await ctx.Containers.FirstOrDefaultAsync(x => x.Id == request.ParentContainerId, cancellationToken);
            if (target is null)
            {
                throw new Exception($"Container with id: {request.ParentContainerId} not found");
            }

            if (target.WarehouseId != container.WarehouseId)
            {
                throw new Exception("Cannot move a container into a different warehouse");
            }

            var ancestorId = target.ParentContainerId;
            while (ancestorId.HasValue)
            {
                if (ancestorId.Value == container.Id)
                {
                    throw new Exception("Cannot move a container into one of its own sub-containers");
                }

                ancestorId = await ctx.Containers
                    .Where(x => x.Id == ancestorId.Value)
                    .Select(x => x.ParentContainerId)
                    .FirstOrDefaultAsync(cancellationToken);
            }

            container.ParentContainerId = target.Id;
            container.CanvasX = null;
            container.CanvasY = null;

            await ctx.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
