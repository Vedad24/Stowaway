using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Domain.Entities.Sales
{
    public enum OrderStatus
    {
        Draft = 1,
        Processing,
        Completed,
        Cancelled,
        Refunded
    }
    [Table("OrderStatus", Schema = "Sales")]
    public class OrderStatusEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public OrderStatus Id { get; set; }
        public string Description { get; set; }
    }
}
