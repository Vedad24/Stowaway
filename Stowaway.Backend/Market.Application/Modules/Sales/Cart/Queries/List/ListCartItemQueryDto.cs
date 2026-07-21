using Market.Domain.Entities.Sales;
using Stowaway.Domain.Entities.Storage;

namespace Market.Application.Modules.Sales.Cart.Queries.List
{
    public class ListCartItemQueryDto
    {
        public required List<CartItemDto> CartItems { get; set; }
    }

    public class CartItemDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public required ContainerTypeEntity ContainerType { get; set; }
        public int Quantity { get; set; }
        public required CartItemStatus CartItemStatus { get; set; }
    }
}