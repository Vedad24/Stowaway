using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Catalog.Products.Commands.Create
{
    public class CreateProductCommandHandler(IAppDbContext Db) : IRequestHandler<CreateProductCommand, int>
    {
        public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            ContainerEntity product = new()
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                StockQuantity = request.StockQuantity ?? 0,
                CategoryId = request.CategoryId,
                IsEnabled = request.isEnabled ?? true,
                IsDeleted = false,
                CreatedAtUtc = DateTime.UtcNow,
            };
            await Db.Products.AddAsync(product,cancellationToken);
            await Db.SaveChangesAsync(cancellationToken);
            return product.Id;
        }
    }
}
