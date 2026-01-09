namespace Stowaway.Application.Modules.Storage.Warehouse.Queries.GetById
{
    public sealed class GetWarehouseQueryByIdDto
    {
        public required int Id {  get; init; }
        public required string Name { get; init; }
    }
}
