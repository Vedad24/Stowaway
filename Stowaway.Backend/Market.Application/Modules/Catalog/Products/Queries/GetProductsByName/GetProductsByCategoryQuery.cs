using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Catalog.Products.Queries.GetProductsByName
{
    public class GetProductsByNameQuery : BasePagedQuery<GetProductsByNameQueryDto>
    {
        public string? ProductName { get; init; }
    }
}
