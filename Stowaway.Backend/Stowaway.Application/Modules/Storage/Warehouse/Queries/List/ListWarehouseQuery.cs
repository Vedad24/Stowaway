namespace Stowaway.Application.Modules.Storage.Warehouse.Queries.List
{
    public sealed class ListWarehouseQuery : BasePagedQuery<ListWarehouseQueryDto>
    {
        public string? Search {  get; init; }
        public string? City { get; init; }
        public string? Address { get; init; }
        public bool? IsEnabled { get; init; }
        public int? MinCapacity { get; init; }
        public int? MaxCapacity { get; init; }
    }
}
