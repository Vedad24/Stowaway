namespace Stowaway.Application.Modules.Storage.Warehouse.Commands.Delete
{
    public class DeleteWarehouseCommand : IRequest<Unit>
    {
        public required int Id { get; set; }
    }
}
