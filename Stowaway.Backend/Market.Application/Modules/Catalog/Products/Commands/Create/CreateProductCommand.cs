using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Catalog.Products.Commands.Create
{
    public class CreateProductCommand: IRequest<int>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int? StockQuantity { get; set; }
        public int CategoryId { get; set; }
        public bool? isEnabled { get; set; }
        
    }
}
