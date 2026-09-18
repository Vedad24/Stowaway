namespace Stowaway.Application.Modules.Storage.Container.Shared
{
    public sealed record ContainerCapacityNode(int Id, string Name, int MaxItems);

    // Items nested inside a sub-container are still physically stored inside every
    // container that transitively holds it, so capacity has to be checked against a
    // whole subtree's usage — and against every ancestor up the chain, since a deeply
    // nested addition counts against all of them — not just what's directly assigned
    // to one container.
    public static class ContainerCapacityHelper
    {
        public static async Task<HashSet<int>> CollectSubtreeIds(IAppDbContext ctx, int rootContainerId, CancellationToken cancellationToken)
        {
            var ids = new HashSet<int> { rootContainerId };
            var frontier = new List<int> { rootContainerId };

            while (frontier.Count > 0)
            {
                var children = await ctx.Containers
                    .Where(c => c.ParentContainerId != null && frontier.Contains(c.ParentContainerId.Value))
                    .Select(c => c.Id)
                    .ToListAsync(cancellationToken);

                frontier = children.Where(ids.Add).ToList();
            }

            return ids;
        }

        public static async Task<int> GetRecursiveItemQuantity(
            IAppDbContext ctx,
            int containerId,
            CancellationToken cancellationToken,
            IReadOnlyCollection<int>? excludeContainerIds = null,
            IReadOnlyCollection<int>? excludeItemIds = null)
        {
            var subtreeIds = await CollectSubtreeIds(ctx, containerId, cancellationToken);
            if (excludeContainerIds is { Count: > 0 })
            {
                subtreeIds.ExceptWith(excludeContainerIds);
            }

            var query = ctx.Item.Where(i => subtreeIds.Contains(i.ContainerId));
            if (excludeItemIds is { Count: > 0 })
            {
                query = query.Where(i => !excludeItemIds.Contains(i.Id));
            }

            return await query.SumAsync(i => (int?)i.Quantity, cancellationToken) ?? 0;
        }

        // Walks from `containerId` up to the warehouse root, inclusive of `containerId` itself.
        public static async Task<List<ContainerCapacityNode>> GetAncestorChain(IAppDbContext ctx, int containerId, CancellationToken cancellationToken)
        {
            var chain = new List<ContainerCapacityNode>();
            int? currentId = containerId;

            while (currentId.HasValue)
            {
                var current = await ctx.Containers
                    .Where(c => c.Id == currentId.Value)
                    .Select(c => new { c.Id, c.Name, c.ParentContainerId, MaxItems = c.ContainerType!.MaxItems })
                    .FirstOrDefaultAsync(cancellationToken);

                if (current is null)
                {
                    break;
                }

                chain.Add(new ContainerCapacityNode(current.Id, current.Name, current.MaxItems));
                currentId = current.ParentContainerId;
            }

            return chain;
        }

        // Validates that adding `quantity` items into `containerId` doesn't overflow that
        // container or any ancestor it's nested inside. `excludeItemIds` should list any
        // item(s) already counted somewhere in the chain that this same quantity replaces
        // (e.g. an item keeping its container but changing quantity, or moving between two
        // containers that share an ancestor).
        public static async Task EnsureItemFits(
            IAppDbContext ctx,
            int containerId,
            int quantity,
            CancellationToken cancellationToken,
            IReadOnlyCollection<int>? excludeItemIds = null)
        {
            var chain = await GetAncestorChain(ctx, containerId, cancellationToken);

            foreach (var node in chain)
            {
                var used = await GetRecursiveItemQuantity(ctx, node.Id, cancellationToken, excludeItemIds: excludeItemIds);
                if (used + quantity > node.MaxItems)
                {
                    throw new ValidationException(
                        $"\"{node.Name}\" can only hold {node.MaxItems} items in total, including nested containers; this would bring it to {used + quantity}.");
                }
            }
        }

        // Same as EnsureItemFits, but for relocating an entire container subtree (e.g.
        // moving a container, or reparenting a deleted container's children) rather than a
        // single item. `movingContainerId`'s whole subtree is excluded from every ancestor's
        // "already used" count so a move within the same branch nets to zero instead of
        // double-counting the items that are simply relocating, not growing in number.
        public static async Task EnsureSubtreeFits(
            IAppDbContext ctx,
            int movingContainerId,
            int targetContainerId,
            CancellationToken cancellationToken)
        {
            var movingSubtreeIds = await CollectSubtreeIds(ctx, movingContainerId, cancellationToken);
            var movedQuantity = await ctx.Item
                .Where(i => movingSubtreeIds.Contains(i.ContainerId))
                .SumAsync(i => (int?)i.Quantity, cancellationToken) ?? 0;

            if (movedQuantity == 0)
            {
                return;
            }

            var chain = await GetAncestorChain(ctx, targetContainerId, cancellationToken);
            foreach (var node in chain)
            {
                var used = await GetRecursiveItemQuantity(ctx, node.Id, cancellationToken, excludeContainerIds: movingSubtreeIds);
                if (used + movedQuantity > node.MaxItems)
                {
                    throw new ValidationException(
                        $"\"{node.Name}\" can only hold {node.MaxItems} items in total, including nested containers; moving this container's contents here would bring it to {used + movedQuantity}.");
                }
            }
        }

        public static async Task EnsureContainerCountFits(
            IAppDbContext ctx,
            int targetContainerId,
            int incomingCount,
            CancellationToken cancellationToken,
            int? excludeContainerId = null)
        {
            var target = await ctx.Containers
                .Where(c => c.Id == targetContainerId)
                .Select(c => new { c.Name, MaxContainers = c.ContainerType!.MaxContainers })
                .FirstOrDefaultAsync(cancellationToken);

            if (target is null)
            {
                return;
            }

            var currentChildCount = await ctx.Containers
                .Where(c => c.ParentContainerId == targetContainerId && (excludeContainerId == null || c.Id != excludeContainerId))
                .CountAsync(cancellationToken);

            if (currentChildCount + incomingCount > target.MaxContainers)
            {
                throw new ValidationException(
                    $"\"{target.Name}\" can only hold {target.MaxContainers} direct child containers; this would bring it to {currentChildCount + incomingCount}.");
            }
        }
    }
}
