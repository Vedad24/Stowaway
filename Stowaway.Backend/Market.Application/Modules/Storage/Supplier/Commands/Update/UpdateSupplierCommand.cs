namespace Stowaway.Application.Modules.Storage.Supplier.Commands.Update
{
    public sealed class UpdateSupplierCommand : IRequest<Unit>
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required int TotalDeliveries { get; set; }
        public required int FailedDeliveries { get; set; }
    }
}
