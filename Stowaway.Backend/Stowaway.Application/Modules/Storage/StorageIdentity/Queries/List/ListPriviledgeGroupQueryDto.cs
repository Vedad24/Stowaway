namespace Stowaway.Application.Modules.Storage.StorageIdentity.Queries.List
{
    public sealed class ListPriviledgeGroupQueryDto
    {
        public required int Id { get; init; }

        public required string Name { get; init; }

        public required int WarehouseId { get; init; }

        public List<int> PriviledgeIds { get; init; } = new();
    }
}
