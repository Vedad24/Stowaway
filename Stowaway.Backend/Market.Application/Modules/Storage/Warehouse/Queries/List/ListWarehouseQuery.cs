namespace Stowaway.Application.Modules.Storage.Warehouse.Queries.List
{
    public sealed class ListWarehouseQuery : BasePagedQuery<ListWarehouseQueryDto>
    {
        public string? Search {  get; init; }
    }
}
