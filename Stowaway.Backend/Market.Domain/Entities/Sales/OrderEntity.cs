using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Domain.Entities.Sales
{
    [Table("Order", Schema = "Sales")]

    public class OrderEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Total { get; set; }

        public int OrderStatusId { get; set; }
        public OrderStatusEntity? OrderStatus { get; set; }
    }
}
