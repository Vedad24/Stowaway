
namespace Stowaway.Application.Modules.Storage.Warehouse.Queries.GetById
{
    public sealed class GetWarehouseQueryByIdHandler(IAppDbContext ctx, IAppCurrentUser appCurrentUser)
        : IRequestHandler<GetWarehouseByIdQuery, GetWarehouseQueryByIdDto>
    {
        public async Task<GetWarehouseQueryByIdDto> Handle(GetWarehouseByIdQuery request, CancellationToken cancellationToken)
        {
            var query = ctx.Warehouses.Where(x => x.Id == request.Id);

            if (!appCurrentUser.IsAdmin)
            {
                query = query.Where(x => ctx.WarehouseUsers.Any(wu => wu.WarehouseId == x.Id && wu.UserId == appCurrentUser.UserId));
            }

            var warehouse = await query
                .Select(x => new GetWarehouseQueryByIdDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    City = x.City,
                    Address = x.Address,
                    Capacity = x.Capacity,
                    isEnabled = x.isEnabled
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (warehouse == null)
            {
                throw new StowawayNotFoundException($"Warehouse with ID : {request.Id} not found");
            }

            return warehouse;
        }
    }
}
