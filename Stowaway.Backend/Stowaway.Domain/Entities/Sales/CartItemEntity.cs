using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Stowaway.Domain.Entities.Storage;

namespace Stowaway.Domain.Entities.Sales
{
    [Table("CartItem", Schema = "Sales")]
    public class CartItemEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public required int WarehouseId { get; set; }
        public ContainerTypeEntity ContainerType { get; set; }
        public int Quantity { get; set; }
        public CartItemStatus CartItemStatus { get; set; }
    }

    public enum CartItemStatus
    {
        InCart,
        SavedForLater
    }
}