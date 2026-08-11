namespace Stowaway.Application.Modules.Storage.Warehouse.Commands.UpdateName
{
    public class UpdateWarehouseNameCommandDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
    }
}
