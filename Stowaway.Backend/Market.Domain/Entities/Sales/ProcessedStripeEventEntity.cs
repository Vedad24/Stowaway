using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stowaway.Domain.Entities.Sales
{
    [Table("ProcessedStripeEvent", Schema = "Sales")]
    public class ProcessedStripeEventEntity
    {
        public string EventId { get; set; } = string.Empty;
        public DateTime ProcessedAtUtc { get; set; }
    }
}
