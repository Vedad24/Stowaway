using Stowaway.Domain.Entities.Sales;

namespace Stowaway.Application.Modules.Sales.Order.Queries.List
{
    public class ListOrdersQueryDto
    {
        public int Id { get; set; }
        public required ListOrdersQueryDtoUser User { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Total { get; set; }
        public DateTime OrderDate { get; set; }
        public required string OrderStatus { get; set; }
    }
    public class ListOrdersQueryDtoUser
    {
        public required string Email { get; set; }
        public required string Name { get; set; }

    }
}