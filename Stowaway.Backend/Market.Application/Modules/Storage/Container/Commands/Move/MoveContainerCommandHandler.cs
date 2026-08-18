using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stowaway.Application.Modules.Storage.Container.Shared;

namespace Stowaway.Application.Modules.Storage.Container.Commands.Move
{
    public class MoveContainerCommandHandler(IAppDbContext ctx) : IRequestHandler<MoveContainerCommand, Unit>
    {
        public async Task<Unit> Handle(MoveContainerCommand request, CancellationToken cancellationToken)
        {
            if (request.Id == request.ParentContainerId)
            {
                throw new StowawayBusinessRuleException("container.self-parent", "A container cannot be placed inside itself");
            }

            var container = await ctx.Containers.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (container is null)
            {
                throw new StowawayNotFoundException($"Container with id: {request.Id} not found");
            }

            var target = await ctx.Containers.FirstOrDefaultAsync(x => x.Id == request.ParentContainerId, cancellationToken);
            if (target is null)
            {
                throw new StowawayNotFoundException($"Container with id: {request.ParentContainerId} not found");
            }

            if (target.WarehouseId != container.WarehouseId)
            {
                throw new StowawayBusinessRuleException("container.cross-warehouse", "Cannot move a container into a different warehouse");
            }

            var containerType = await ctx.ContainerTypes.FirstOrDefaultAsync(t => t.Id == container.ContainerTypeId, cancellationToken);
            var targetType = await ctx.ContainerTypes.FirstOrDefaultAsync(t => t.Id == target.ContainerTypeId, cancellationToken);
            if (containerType is not null && targetType is not null &&
                (containerType.MaxItems >= targetType.MaxItems || containerType.MaxContainers >= targetType.MaxContainers))
            {
                throw new ValidationException(
                    $"This container type (max {containerType.MaxItems} items / {containerType.MaxContainers} containers) must be strictly smaller than the target container's capacity (max {targetType.MaxItems} items / {targetType.MaxContainers} containers) — same-size containers can't nest either.");
            }

            var ancestorId = target.ParentContainerId;
            while (ancestorId.HasValue)
            {
                if (ancestorId.Value == container.Id)
                {
                    throw new StowawayBusinessRuleException("container.circular-nesting", "Cannot move a container into one of its own sub-containers");
                }

                ancestorId = await ctx.Containers
                    .Where(x => x.Id == ancestorId.Value)
                    .Select(x => x.ParentContainerId)
                    .FirstOrDefaultAsync(cancellationToken);
            }

            await ContainerCapacityHelper.EnsureSubtreeFits(ctx, container.Id, target.Id, cancellationToken);

            container.ParentContainerId = target.Id;
            container.CanvasX = null;
            container.CanvasY = null;

            await ctx.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
