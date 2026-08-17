using Stowaway.Domain.Entities.Storage;
using Stowaway.Application.Modules.Storage.Container.Commands.Create;
namespace Stowaway.Application.Modules.Storage.Container.Commands.Create
{
    public class CreateContainerCommandHandler(IAppDbContext ctx) : IRequestHandler<CreateContainerCommand, int>
    {
        public async Task<int> Handle(CreateContainerCommand request, CancellationToken cancellationToken)
        {
            var normalized = request.Name?.Trim();

            if (string.IsNullOrWhiteSpace(normalized))
            {
                throw new ValidationException("Name is required.");
            }

            var warehouse = await ctx.Warehouses.FirstOrDefaultAsync(x => x.Id == request.WarehouseId, cancellationToken);
            if (warehouse is null)
            {
                throw new Exception($"Warehouse with id: {request.WarehouseId} not found");
            }

            var containerType = await ctx.ContainerTypes.FirstOrDefaultAsync(x => x.Id == request.ContainerTypeId, cancellationToken);
            if (containerType is null)
            {
                throw new Exception($"Container type with id: {request.ContainerTypeId} not found");
            }

            if (request.ParentContainerId.HasValue)
            {
                var parent = await ctx.Containers.FirstOrDefaultAsync(x => x.Id == request.ParentContainerId.Value, cancellationToken);
                if (parent is null)
                {
                    throw new Exception($"Container with id: {request.ParentContainerId.Value} not found");
                }

                if (parent.WarehouseId != request.WarehouseId)
                {
                    throw new Exception("Parent container belongs to a different warehouse");
                }

                var parentType = await ctx.ContainerTypes.FirstOrDefaultAsync(t => t.Id == parent.ContainerTypeId, cancellationToken);
                if (parentType is not null && (containerType.MaxItems >= parentType.MaxItems || containerType.MaxContainers >= parentType.MaxContainers))
                {
                    throw new ValidationException(
                        $"This container type (max {containerType.MaxItems} items / {containerType.MaxContainers} containers) must be strictly smaller than the parent container's capacity (max {parentType.MaxItems} items / {parentType.MaxContainers} containers) — same-size containers can't nest either.");
                }
            }

            var container = new ContainerEntity
            {
                Name = normalized,
                ContainerTypeId = request.ContainerTypeId,
                WarehouseId = request.WarehouseId,
                ParentContainerId = request.ParentContainerId,
            };

            ctx.Containers.Add(container);
            await ctx.SaveChangesAsync(cancellationToken);

            return container.Id;
        }
    }
}
