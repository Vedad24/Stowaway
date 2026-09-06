namespace Stowaway.Application.Modules.Storage.Warehouse.Commands.Create
{
    public class CreateWarehouseCommand : IRequest<int>
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string City { get; set; }
        public required string Address { get; set; }
        public required int Capacity { get; set; }
        public required bool isEnabled { get; set; }
    }
}
