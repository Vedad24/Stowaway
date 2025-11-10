using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Catalog.Products.Queries.GetProductById
{
    public sealed class GetProductsByIdQueryHandler(IAppDbContext Db) : IRequestHandler<GetProductByIdQuery,GetProductByIdQueryDto>
    {
        public async Task<GetProductByIdQueryDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {


            var product = await Db.Products.Include(p => p.Category).AsNoTracking().FirstOrDefaultAsync(p => p.Id == request.Id);
            if (product is null)
                throw new MarketNotFoundException($"Nema tog proizvoda -> Id : {request.Id}");
            var result = new GetProductByIdQueryDto()
            {
                Name = product.Name,
                Description = product.Description,
                CategoryName = product.Category.Name,
                Price = product.Price
            };

            return result;

        }
    }
}
