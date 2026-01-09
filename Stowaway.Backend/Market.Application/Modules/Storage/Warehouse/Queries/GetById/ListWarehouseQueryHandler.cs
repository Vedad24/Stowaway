
namespace Stowaway.Application.Modules.Storage.Warehouse.Queries.GetById
{
    public sealed class GetWarehouseQueryByIdHandler(IAppDbContext ctx)
        : IRequestHandler<GetWarehouseByIdQuery, GetWarehouseQueryByIdDto>
    {
        public async Task<GetWarehouseQueryByIdDto> Handle(GetWarehouseByIdQuery request, CancellationToken cancellationToken)
        {
            var warehouse = await ctx.Warehouses
                .Where(x => x.Id == request.Id)
                .Select(x => new GetWarehouseQueryByIdDto 
                {
                    Id = x.Id,
                    Name = x.Name,
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (warehouse == null)
            {
                throw new Exception($"Warehouse with ID : {request.Id} not found");
            }

            return warehouse;
        }
    }
}
