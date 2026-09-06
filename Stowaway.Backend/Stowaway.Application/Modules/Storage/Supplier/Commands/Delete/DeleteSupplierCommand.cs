namespace Stowaway.Application.Modules.Storage.Supplier.Commands.Delete
{
    public class DeleteSupplierCommand : IRequest<Unit>
    {
        public required int Id { get; set; }
    }
}
