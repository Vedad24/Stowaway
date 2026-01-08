namespace Stowaway.Application.Modules.Storage.Warehouse.Commands.Create
{
    public class CreateWarehouseCommand : IRequest<int>
    {
        public required string Name { get; set; }
    }
}
