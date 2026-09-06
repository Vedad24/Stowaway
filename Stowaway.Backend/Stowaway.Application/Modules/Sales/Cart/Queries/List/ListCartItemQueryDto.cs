using Stowaway.Domain.Entities.Sales;
using Stowaway.Domain.Entities.Storage;

namespace Stowaway.Application.Modules.Sales.Cart.Queries.List
{
    public class ListCartItemQueryDto
    {
        public required List<CartItemDto> CartItems { get; set; }
    }

    public class CartItemDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int WarehouseId { get; set; }
        public required string WarehouseName { get; set; }
        public required CartItemContainerTypeDto ContainerType { get; set; }
        public int Quantity { get; set; }
        public required CartItemStatus CartItemStatus { get; set; }
    }

    public class CartItemContainerTypeDto : ContainerTypeEntity
    {
        
        public CartItemContainerTypeDto(ContainerTypeEntity obj)
        {
            this.Id = obj.Id;
            this.MaxContainers = obj.MaxContainers;
            this.MaxItems = obj.MaxItems;
            this.Price = obj.Price;
            this.DisplayName = obj.ToString();

        }
        public string DisplayName {get; set;}
    }
}