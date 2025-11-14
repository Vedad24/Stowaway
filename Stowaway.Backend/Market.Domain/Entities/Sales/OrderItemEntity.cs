using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stowaway.Domain.Entities.Storage;

namespace Stowaway.Domain.Entities.Sales
{
    [Table("OrderItem", Schema = "Sales")]

    public class OrderItemEntity
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public OrderEntity? Order { get; set; }
        public int ContainerTypeId { get; set; }
        public ContainerTypeEntity? ContainerType { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Total { get; set; }
        public decimal Discount { get; set; }

    }
}
