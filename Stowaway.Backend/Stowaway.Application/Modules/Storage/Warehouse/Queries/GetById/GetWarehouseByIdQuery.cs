namespace Stowaway.Application.Modules.Storage.Warehouse.Queries.GetById
{
    public sealed class GetWarehouseByIdQuery : IRequest<GetWarehouseQueryByIdDto>
    {
        public int Id {  get; set; }
    }
}
