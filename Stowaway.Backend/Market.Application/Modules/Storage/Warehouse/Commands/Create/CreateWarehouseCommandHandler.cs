
using Stowaway.Domain.Entities.Storage;

namespace Stowaway.Application.Modules.Storage.Warehouse.Commands.Create
{
    public class CreateWarehouseCommandHandler(IAppDbContext ctx) : IRequestHandler<CreateWarehouseCommand, int>
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
                throw new Exception("Warehouse with this name already exists.");
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

            ctx.Warehouses.Add(warehouse);
            await ctx.SaveChangesAsync(cancellationToken);

            return warehouse.Id;
        }
    }
}
