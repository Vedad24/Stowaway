using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Catalog.Products.Queries.GetProductsByCategory
{
    public class GetProductsByCategoryQuery : BasePagedQuery<GetProductsByCategoryQueryDto>
    {
        public string? CategoryName { get; set; }
    }
}
