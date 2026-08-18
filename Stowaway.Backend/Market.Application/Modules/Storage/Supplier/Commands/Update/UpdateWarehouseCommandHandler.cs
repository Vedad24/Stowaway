
using Stowaway.Domain.Entities.Storage;

namespace Stowaway.Application.Modules.Storage.Supplier.Commands.Update
{
    public class UpdateSupplierCommandHandler(IAppDbContext ctx) : IRequestHandler<UpdateSupplierCommand, Unit>
    {
        public async Task<Unit> Handle(UpdateSupplierCommand request, CancellationToken cancellationToken)
        {
            var supplier = await ctx.Suppliers
                .Where(x => x.Id == request.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (supplier == null) {
                throw new StowawayNotFoundException($"Supplier with id: {request.Id} not found");
            }

            var exist = await ctx.Suppliers
                .AnyAsync(x => x.Id != request.Id && x.Name.ToLower() == request.Name.ToLower(), cancellationToken);

            if (exist)
            {
                throw new StowawayConflictException("Supplier with this name already exists");
            }

            supplier.Name = request.Name.Trim();
            supplier.Description = request.Description.Trim();
            supplier.Address = request.Address.Trim();
            supplier.TotalDeliveries = request.TotalDeliveries;
            supplier.FailedDeliveries = request.FailedDeliveries;

            await ctx.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
