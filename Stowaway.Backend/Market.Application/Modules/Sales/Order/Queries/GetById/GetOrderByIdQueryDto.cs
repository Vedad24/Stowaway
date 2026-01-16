using Stowaway.Application.Modules.Sales.Order.Queries.List;
using Stowaway.Domain.Entities.Sales;
using Stowaway.Domain.Entities.Storage;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Application.Modules.Sales.Order.Queries.GetById
{
    public class GetOrderByIdQueryDto
    {
        public required GetOrderByIdQueryDtoUser User { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Total { get; set; }
        public DateTime OrderDate { get; set; }
        public required string OrderStatus { get; set; }
        public List<GetOrderByIdQueryDtoOrderItem> Items { get; set; }
    }

    public class GetOrderByIdQueryDtoOrderItem
    {
        public string ContainerType { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }
    }

    public class GetOrderByIdQueryDtoUser
    {
        public required string Email { get; set; }
        public required string Name { get; set; }
    }
}
