
using Stowaway.Application.Modules.Storage.StorageIdentity;
using Stowaway.Domain.Entities.Storage;

namespace Stowaway.Application.Modules.Storage.Warehouse.Commands.Create
{
    public class CreateWarehouseCommandHandler(IAppDbContext ctx, IAppCurrentUser appCurrentUser) : IRequestHandler<CreateWarehouseCommand, int>
    {
        public async Task<int> Handle(CreateWarehouseCommand request, CancellationToken cancellationToken)
        {
            var normalized = request.Name?.Trim();

            if (string.IsNullOrWhiteSpace(normalized))
            {
                throw new ValidationException("Name is required.");
            }

            bool exists = await ctx.Warehouses.AnyAsync(x => x.Name == normalized, cancellationToken);

            if (exists) {
                throw new StowawayConflictException("Warehouse with this name already exists.");
            }

            var warehouse = new WarehouseEntity
            {
                Name = normalized,
                Description = request.Description,
                City = request.City,
                Address = request.Address,
                Capacity = request.Capacity,
                isEnabled = request.isEnabled
            };

            if (appCurrentUser.UserId is null)
            {
                throw new StowawayUnauthorizedException("Current user could not be resolved.");
            }

            ctx.Warehouses.Add(warehouse);
            await ctx.SaveChangesAsync(cancellationToken);

            await WarehouseOwnerProvisioning.EnsureOwnerAccessAsync(ctx, warehouse.Id, appCurrentUser.UserId.Value, cancellationToken);

            return warehouse.Id;
        }
    }
}
