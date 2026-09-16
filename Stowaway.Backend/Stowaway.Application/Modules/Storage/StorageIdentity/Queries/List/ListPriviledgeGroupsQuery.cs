namespace Stowaway.Application.Modules.Storage.StorageIdentity.Queries.List
{
    public sealed class ListPriviledgeGroupsQuery : BasePagedQuery<ListPriviledgeGroupQueryDto>
    {
        public int? WarehouseId { get; init; }
    }
}
