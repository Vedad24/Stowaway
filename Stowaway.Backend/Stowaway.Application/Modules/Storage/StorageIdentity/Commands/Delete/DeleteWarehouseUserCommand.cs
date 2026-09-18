namespace Stowaway.Application.Modules.Storage.StorageIdentity.Commands.Delete
{
    public class DeleteWarehouseUserCommand : IRequest<Unit>
    {
        public required int WarehouseId { get; set; }
        public required int UserId { get; set; }
        public required int PriviledgeGroupId { get; set; }
    }
}
