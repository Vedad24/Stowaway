namespace Stowaway.Application.Modules.Storage.Warehouse.Commands.Update
{
    public sealed class UpdateWarehouseCommand : IRequest<Unit>
    {
        public int Id { get; set; }
        public required string Name { get; set; }
    }
}
