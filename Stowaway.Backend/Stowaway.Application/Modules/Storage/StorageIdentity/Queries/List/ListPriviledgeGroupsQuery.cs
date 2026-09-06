namespace Stowaway.Application.Modules.Storage.StorageIdentity.Queries.List
{
    public sealed class ListPriviledgeGroupsQuery : IRequest<List<ListPriviledgeGroupQueryDto>>
    {
        public int? WarehouseId { get; init; }
    }
}
