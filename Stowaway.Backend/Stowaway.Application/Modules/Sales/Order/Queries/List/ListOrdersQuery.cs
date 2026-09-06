using Stowaway.Application.Common;
using Stowaway.Domain.Entities.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Application.Modules.Sales.Order.Queries.List
{
    public class ListOrdersQuery : BasePagedQuery<ListOrdersQueryDto>
    {
        public string? SearchByUserEmail {  get; set; }
        public string? SearchByUserName { get; set; }
        public string? SearchByWarehouseName { get; set; }
        public OrderStatus? SearchByStatus { get; set; }

        public DateTime? CreateTimeMin { get; set; }
        public DateTime? CreateTimeMax { get; set; }

    }
}
