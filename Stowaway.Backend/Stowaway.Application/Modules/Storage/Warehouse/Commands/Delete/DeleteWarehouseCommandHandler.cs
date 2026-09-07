
using Stowaway.Domain.Entities.Storage;

namespace Stowaway.Application.Modules.Storage.Warehouse.Commands.Delete
{
    public class DeleteWarehouseCommandHandler(IAppDbContext ctx) : IRequestHandler<DeleteWarehouseCommand, Unit>
    {
        public async Task<Unit> Handle(DeleteWarehouseCommand request, CancellationToken cancellationToken)
        {
            var warehouse = await ctx.Warehouses.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (warehouse == null)
            {
                throw new StowawayNotFoundException($"Warehouse with id: {request.Id} is not found");
            }

            var hasContainers = await ctx.Containers.AnyAsync(x => x.WarehouseId == warehouse.Id, cancellationToken);
            if (hasContainers)
            {
                throw new ValidationException("Warehouse has containers. Delete or empty them before deleting the warehouse.");
            }

            var hasOrderHistory = await ctx.OrderItems.AnyAsync(x => x.WarehouseId == warehouse.Id, cancellationToken);
            if (hasOrderHistory)
            {
                throw new ValidationException("Warehouse has order history and cannot be deleted.");
            }

            var hasWarehouseUsers = await ctx.WarehouseUsers.AnyAsync(x => x.WarehouseId == warehouse.Id, cancellationToken);
            if (hasWarehouseUsers)
            {
                throw new ValidationException("Warehouse has assigned users. Remove them before deleting the warehouse.");
            }

            var hasPriviledgeGroups = await ctx.PriviledgeGroups.AnyAsync(x => x.WarehouseId == warehouse.Id, cancellationToken);
            if (hasPriviledgeGroups)
            {
                throw new ValidationException("Warehouse has privilege groups. Delete them before deleting the warehouse.");
            }

            ctx.Warehouses.Remove(warehouse);
            await ctx.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
