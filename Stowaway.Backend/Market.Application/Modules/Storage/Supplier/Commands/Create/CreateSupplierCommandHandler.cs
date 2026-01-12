using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stowaway.Domain.Entities.Storage;

namespace Stowaway.Application.Modules.Storage.Supplier.Commands.Create
{
    public class CreateSupplierCommandHandler(IAppDbContext ctx)
        : IRequestHandler<CreateSupplierCommand, int>
    {
        public async Task<int> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
        {
            var normalized = request.Name?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(normalized))
            {
                throw new Exception("Name is required");
            }

            bool exists = await ctx.Suppliers.AnyAsync(x => x.Name == normalized, cancellationToken);

            if (exists)
            {
                throw new Exception("Supplier already exists with this name");
            }

            var supplier = new SupplierEntity
            {
                Name = normalized,
                Description = request.Description,
                TotalDeliveries = request.TotalDeliveries,
                FailedDeliveries = request.FailedDeliveries,
            };

            ctx.Suppliers.Add(supplier);
            await ctx.SaveChangesAsync(cancellationToken);

            return supplier.Id;
        }
    }
}
