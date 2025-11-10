using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Catalog.Products.Queries.GetProductById
{
    public class GetProductByIdQuery : IRequest<GetProductByIdQueryDto>
    {
        public int Id { get; set; }
    }
}
