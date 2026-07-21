using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Market.Application.Modules.Sales.Cart.Queries.List
{
    public class ListCartItemsQuery : IRequest<ListCartItemQueryDto>
    {
        public int UserId { get; set; }
    }
}