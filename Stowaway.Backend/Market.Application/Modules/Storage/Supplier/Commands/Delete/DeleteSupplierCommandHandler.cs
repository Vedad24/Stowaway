
using Stowaway.Application.Modules.Storage.Supplier.Commands.Delete;
using Stowaway.Domain.Entities.Storage;

namespace Stowaway.Application.Modules.Storage.Supplier.Commands.Delete
{
    public class DeleteSupplierCommandHandler(IAppDbContext ctx) : IRequestHandler<DeleteSupplierCommand, Unit>
    {
        public async Task<Unit> Handle(DeleteSupplierCommand request, CancellationToken cancellationToken)
        {
            var supplier = await ctx.Suppliers.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (supplier == null)
            {
                throw new Exception($"Supplier with id: {request.Id} is not found");
            }

            ctx.Suppliers.Remove(supplier);
            await ctx.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
