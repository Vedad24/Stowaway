namespace Stowaway.Application.Modules.Storage.Container.Shared
{
    public sealed record ContainerCapacityNode(int Id, string Name, int MaxItems);

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

        public static async Task<Dictionary<int, int>> GetRecursiveItemQuantities(
            IAppDbContext ctx,
            IReadOnlyCollection<int> rootContainerIds,
            CancellationToken cancellationToken)
        {
            var roots = rootContainerIds.Distinct().ToList();
            var totals = roots.ToDictionary(id => id, _ => 0);

            if (roots.Count == 0)
            {
                return totals;
            }

            var owner = roots.ToDictionary(id => id, id => id);
            var allIds = new HashSet<int>(roots);
            var frontier = roots;

            while (frontier.Count > 0)
            {
                var children = await ctx.Containers
                    .Where(c => c.ParentContainerId != null && frontier.Contains(c.ParentContainerId.Value))
                    .Select(c => new { c.Id, ParentId = c.ParentContainerId!.Value })
                    .ToListAsync(cancellationToken);

                var nextFrontier = new List<int>();
                foreach (var child in children)
                {
                    if (!allIds.Add(child.Id))
                    {
                        continue;
                    }

                    owner[child.Id] = owner[child.ParentId];
                    nextFrontier.Add(child.Id);
                }

                frontier = nextFrontier;
            }

            var sumsByContainer = await ctx.Item
                .Where(i => allIds.Contains(i.ContainerId))
                .GroupBy(i => i.ContainerId)
                .Select(g => new { ContainerId = g.Key, Sum = g.Sum(i => i.Quantity) })
                .ToListAsync(cancellationToken);

            foreach (var entry in sumsByContainer)
            {
                totals[owner[entry.ContainerId]] += entry.Sum;
            }

            return totals;
        }

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
