namespace Stowaway.Application.Modules.Storage.Warehouse.Queries.GetById
{
    public sealed class GetWarehouseQueryByIdDto
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
