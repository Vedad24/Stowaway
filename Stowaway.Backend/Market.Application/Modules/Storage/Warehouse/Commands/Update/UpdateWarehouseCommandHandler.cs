
using Stowaway.Domain.Entities.Storage;

namespace Stowaway.Application.Modules.Storage.Warehouse.Commands.Update
{
    public class UpdateWarehouseCommandHandler(IAppDbContext ctx) : IRequestHandler<UpdateWarehouseCommand, Unit>
    {
        public async Task<Unit> Handle(UpdateWarehouseCommand request, CancellationToken cancellationToken)
        {
            var warehouse = await ctx.Warehouses
                .Where(x => x.Id == request.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (warehouse == null) {
                throw new Exception($"Warehouse with id: {request.Id} not found");
            }

            var exist = await ctx.Warehouses
                .AnyAsync(x => x.Id != request.Id && x.Name.ToLower() == request.Name.ToLower(), cancellationToken);

            if (exist)
            {
                throw new Exception("Warehouse with this name already exists");
            }

            warehouse.Name = request.Name.Trim();
            warehouse.Description = request.Description.Trim();
            warehouse.City = request.City.Trim();
            warehouse.Address = request.Address.Trim();
            warehouse.Capacity = request.Capacity;
            warehouse.isEnabled = request.isEnabled;

            await ctx.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
