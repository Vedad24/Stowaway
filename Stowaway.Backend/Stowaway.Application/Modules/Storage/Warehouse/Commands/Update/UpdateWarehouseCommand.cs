namespace Stowaway.Application.Modules.Storage.Warehouse.Commands.Update
{
    public sealed class UpdateWarehouseCommand : IRequest<Unit>
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string City { get; set; }
        public required string Address { get; set; }
        public required int Capacity { get; set; }
        public required bool isEnabled { get; set; }
    }
}
