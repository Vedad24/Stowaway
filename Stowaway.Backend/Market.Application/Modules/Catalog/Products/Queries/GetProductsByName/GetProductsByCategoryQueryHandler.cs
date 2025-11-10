using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Catalog.Products.Queries.GetProductsByName
{
    public sealed class GetProductsByNameQueryHandler(IAppDbContext Db) : IRequestHandler<GetProductsByNameQuery, PageResult<GetProductsByNameQueryDto>>
    {
        public async Task<PageResult<GetProductsByNameQueryDto>> Handle(GetProductsByNameQuery request, CancellationToken cancellationToken)
        {
            var allproducts = Db.Products.Include(p => p.Category).AsNoTracking();
            if (request.ProductName is not null)
                allproducts = allproducts.Where(p => p.Name.ToLower().Contains(request.ProductName.ToLower())).AsNoTracking();

            var result = allproducts.Select(p => new GetProductsByNameQueryDto
            {
                Name = p.Name,
                Price = p.Price,
                CategoryName = p.Category.Name
            });
            return await PageResult<GetProductsByNameQueryDto>.FromQueryableAsync(result, request.Paging, cancellationToken);

        }
    }
}
