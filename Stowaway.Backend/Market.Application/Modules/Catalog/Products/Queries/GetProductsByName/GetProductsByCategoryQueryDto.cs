using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Catalog.Products.Queries.GetProductsByName
{
    public class GetProductsByNameQueryDto
    {
        public string Name { get; init; }
        public string CategoryName { get; init; }
        public decimal Price { get; init; }
    }
}
