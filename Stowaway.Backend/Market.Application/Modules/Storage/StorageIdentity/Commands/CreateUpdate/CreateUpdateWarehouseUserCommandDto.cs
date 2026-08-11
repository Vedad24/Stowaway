namespace Stowaway.Application.Modules.Storage.StorageIdentity.Commands.CreateUpdate
{
    public sealed class CreateUpdateWarehouseUserCommandDto
    {
        public required int UserId { get; init; }
        public required int WarehouseId { get; init; }
        public required int PriviledgeGroupId { get; init; }
    }
}
