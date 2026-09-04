using Market.Domain.Common;
using Market.Domain.Entities.Identity;
using Stowaway.Domain.Entities.Storage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Domain.Entities.Sales
{
    [Table("Order", Schema = "Sales")]

    public class OrderEntity : BaseEntity
    {
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public UserEntity? User { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Total { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus OrderStatusId { get; set; }
        public OrderStatusEntity? OrderStatus { get; set; }
        
        // Stripe-specific columns
        public string? PaymentIntentId { get; set; }
        public string? StripeSessionId { get; set; }
        
        public List<OrderItemEntity>? OrderItems { get; set; }
    }
}
