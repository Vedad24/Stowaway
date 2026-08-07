namespace Stowaway.Application.Modules.Storage.Container.Commands.Delete
{
    public class DeleteContainerCommandHandler(IAppDbContext ctx) : IRequestHandler<DeleteContainerCommand, Unit>
    {
        public async Task<Unit> Handle(DeleteContainerCommand request, CancellationToken cancellationToken)
        {
            var container = await ctx.Containers.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (container is null)
            {
                throw new Exception($"Container with id: {request.Id} not found");
            }

            var childContainers = await ctx.Containers
                .Where(x => x.ParentContainerId == container.Id)
                .ToListAsync(cancellationToken);
            var childItems = await ctx.Item
                .Where(x => x.ContainerId == container.Id)
                .ToListAsync(cancellationToken);

            if (childContainers.Count == 0 && childItems.Count == 0)
            {
                ctx.Containers.Remove(container);
                await ctx.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }

            if (request.MoveContentsToContainerId.HasValue)
            {
                var target = await ctx.Containers.FirstOrDefaultAsync(x => x.Id == request.MoveContentsToContainerId.Value, cancellationToken);
                if (target is null)
                {
                    throw new Exception($"Container with id: {request.MoveContentsToContainerId.Value} not found");
                }
                if (target.Id == container.Id)
                {
                    throw new ValidationException("Cannot move contents into the container being deleted.");
                }
                if (target.WarehouseId != container.WarehouseId)
                {
                    throw new ValidationException("Cannot move contents into a container from a different warehouse.");
                }

                var descendantIds = await CollectDescendantIds(container.Id, cancellationToken);
                if (descendantIds.Contains(target.Id))
                {
                    throw new ValidationException("Cannot move contents into one of the container's own sub-containers.");
                }

                foreach (var child in childContainers)
                {
                    child.ParentContainerId = target.Id;
                    child.CanvasX = null;
                    child.CanvasY = null;
                }
                foreach (var item in childItems)
                {
                    item.ContainerId = target.Id;
                    item.CanvasX = null;
                    item.CanvasY = null;
                }

                ctx.Containers.Remove(container);
                await ctx.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }

            if (request.DeleteContents)
            {
                await DeleteChildrenRecursively(container.Id, cancellationToken);
                ctx.Containers.Remove(container);
                await ctx.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }

            throw new ValidationException("Container is not empty. Specify whether to delete its contents or move them to another container.");
        }

        // Items cascade-delete with their container at the DB level, but sub-containers
        // don't — deleting is done explicitly, depth-first, so neither relies on that
        // default behavior and both stay consistent with each other.
        private async Task<HashSet<int>> CollectDescendantIds(int containerId, CancellationToken cancellationToken)
        {
            var descendantIds = new HashSet<int>();
            var frontier = new List<int> { containerId };

            while (frontier.Count > 0)
            {
                var children = await ctx.Containers
                    .Where(x => x.ParentContainerId != null && frontier.Contains(x.ParentContainerId.Value))
                    .Select(x => x.Id)
                    .ToListAsync(cancellationToken);

                frontier = children.Where(id => descendantIds.Add(id)).ToList();
            }

            return descendantIds;
        }

        private async Task DeleteChildrenRecursively(int containerId, CancellationToken cancellationToken)
        {
            var childContainers = await ctx.Containers
                .Where(x => x.ParentContainerId == containerId)
                .ToListAsync(cancellationToken);

            foreach (var child in childContainers)
            {
                await DeleteChildrenRecursively(child.Id, cancellationToken);
            }

            var childItems = await ctx.Item.Where(x => x.ContainerId == containerId).ToListAsync(cancellationToken);
            ctx.Item.RemoveRange(childItems);
            ctx.Containers.RemoveRange(childContainers);
        }
    }
}
