namespace Stowaway.Application.Modules.Storage.StorageIdentity.Commands.Create
{
    public sealed class CreatePriviledgeGroupCommand : IRequest<int>
    {
        public required string Name { get; init; }

        public int WarehouseId { get; init; }

        public List<int> PriviledgeIds { get; init; } = new();
    }
}
