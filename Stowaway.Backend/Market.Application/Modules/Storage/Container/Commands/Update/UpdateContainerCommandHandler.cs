using Stowaway.Application.Modules.Storage.Container.Shared;

namespace Stowaway.Application.Modules.Storage.Container.Commands.Update
{
    public class UpdateContainerCommandHandler(IAppDbContext ctx) : IRequestHandler<UpdateContainerCommand, Unit>
    {
        public async Task<Unit> Handle(UpdateContainerCommand request, CancellationToken cancellationToken)
        {
            var normalized = request.Name?.Trim();

            if (string.IsNullOrWhiteSpace(normalized))
            {
                throw new ValidationException("Name is required.");
            }

            var container = await ctx.Containers.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (container is null)
            {
                throw new Exception($"Container with id: {request.Id} not found");
            }

            var containerType = await ctx.ContainerTypes.FirstOrDefaultAsync(x => x.Id == request.ContainerTypeId, cancellationToken);
            if (containerType is null)
            {
                throw new Exception($"Container type with id: {request.ContainerTypeId} not found");
            }

            if (container.ParentContainerId.HasValue)
            {
                var parentType = await ctx.Containers
                    .Where(x => x.Id == container.ParentContainerId.Value)
                    .Select(x => x.ContainerType)
                    .FirstOrDefaultAsync(cancellationToken);

                if (parentType is not null && (containerType.MaxItems >= parentType.MaxItems || containerType.MaxContainers >= parentType.MaxContainers))
                {
                    throw new ValidationException(
                        $"This container type (max {containerType.MaxItems} items / {containerType.MaxContainers} containers) must be strictly smaller than the parent container's capacity (max {parentType.MaxItems} items / {parentType.MaxContainers} containers) — same-size containers can't nest either.");
                }
            }

            var oversizedChildType = await ctx.Containers
                .Where(x => x.ParentContainerId == container.Id)
                .Select(x => x.ContainerType)
                .Where(t => t != null && (t.MaxItems >= containerType.MaxItems || t.MaxContainers >= containerType.MaxContainers))
                .FirstOrDefaultAsync(cancellationToken);

            if (oversizedChildType is not null)
            {
                throw new ValidationException(
                    $"Cannot shrink to a type with max {containerType.MaxItems} items / {containerType.MaxContainers} containers — it already holds a container that needs max {oversizedChildType.MaxItems} items / {oversizedChildType.MaxContainers} containers.");
            }

            // Relabeling the type doesn't move any items, so only this container's own new
            // cap needs checking against what it (and everything nested inside it) already holds.
            var recursiveUsedItems = await ContainerCapacityHelper.GetRecursiveItemQuantity(ctx, container.Id, cancellationToken);
            if (recursiveUsedItems > containerType.MaxItems)
            {
                throw new ValidationException(
                    $"Cannot shrink to a type with max {containerType.MaxItems} items — this container already holds {recursiveUsedItems} items, including nested containers.");
            }

            container.Name = normalized;
            container.ContainerTypeId = request.ContainerTypeId;

            await ctx.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
