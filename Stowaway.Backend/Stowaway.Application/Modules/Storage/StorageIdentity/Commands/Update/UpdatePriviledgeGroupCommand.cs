namespace Stowaway.Application.Modules.Storage.StorageIdentity.Commands.Update
{
    public sealed class UpdatePriviledgeGroupCommand : IRequest<int>
    {
        public int? PriviledgeId { get; set; }
        public required string Name { get; init; }

        public int WarehouseId { get; init; }

        public List<int> PriviledgeIds { get; init; } = new();
    }
}
