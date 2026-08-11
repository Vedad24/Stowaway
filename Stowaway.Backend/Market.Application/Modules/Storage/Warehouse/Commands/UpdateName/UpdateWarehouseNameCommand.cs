namespace Stowaway.Application.Modules.Storage.Warehouse.Commands.UpdateName
{
    public sealed class UpdateWarehouseNameCommand : IRequest<UpdateWarehouseNameCommandDto>
    {
        public int Id { get; set; }
        public required string Name { get; set; }
    }
}
