namespace Stowaway.Application.Modules.Storage.Warehouse.Queries.List
{
    public sealed class ListWarehouseQueryDto
    {
        public required int Id {  get; init; }
        public required string Name { get; init; }
        public required string Description { get; init; }
        public required string City { get; init; }
        public required string Address { get; init; }
        public required int Capacity { get; init; }
        public required bool isEnabled { get; init; }
    }
}
