using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Catalog.Products.Queries.GetProductsByCategory
{
    public sealed class GetProductsByCategoryQueryHandler(IAppDbContext Db) : IRequestHandler<GetProductsByCategoryQuery, PageResult<GetProductsByCategoryQueryDto>>
    {
        public async Task<PageResult<GetProductsByCategoryQueryDto>> Handle(GetProductsByCategoryQuery request, CancellationToken cancellationToken)
        {
            var allproducts = Db.Products.Include(p => p.Category).AsNoTracking();
            if (request.CategoryName is not null)
                allproducts = allproducts.Where(p => p.Category.Name.ToLower().Contains(request.CategoryName.ToLower())).AsNoTracking();

            var result = allproducts.Select(p => new GetProductsByCategoryQueryDto
            {
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                CategoryName = p.Category.Name
            });
            return await PageResult<GetProductsByCategoryQueryDto>.FromQueryableAsync(result, request.Paging, cancellationToken);

        }
    }
}
