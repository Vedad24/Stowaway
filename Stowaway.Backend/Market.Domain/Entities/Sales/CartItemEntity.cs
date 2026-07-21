using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Stowaway.Domain.Entities.Storage;

namespace Market.Domain.Entities.Sales
{
    public class CartItemEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
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