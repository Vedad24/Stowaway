
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
                throw new Exception($"Warehouse with id: {request.Id} is not found");
            }

            ctx.Warehouses.Remove(warehouse);
            await ctx.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
