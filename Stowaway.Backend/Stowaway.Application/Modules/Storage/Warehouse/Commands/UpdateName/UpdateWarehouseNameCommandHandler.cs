namespace Stowaway.Application.Modules.Storage.Warehouse.Commands.UpdateName
{
    public class UpdateWarehouseNameCommandHandler(IAppDbContext ctx) : IRequestHandler<UpdateWarehouseNameCommand, UpdateWarehouseNameCommandDto>
    {
        public async Task<UpdateWarehouseNameCommandDto> Handle(UpdateWarehouseNameCommand request, CancellationToken cancellationToken)
        {
            var warehouse = await ctx.Warehouses
                .Where(x => x.Id == request.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (warehouse == null) {
                throw new StowawayNotFoundException($"Warehouse with id: {request.Id} not found");
            }

            var exist = await ctx.Warehouses
                .AnyAsync(x => x.Id != request.Id && x.Name.ToLower() == request.Name.ToLower(), cancellationToken);

            if (exist)
            {
                throw new StowawayConflictException("Warehouse with this name already exists");
            }

            warehouse.Name = request.Name.Trim();

            await ctx.SaveChangesAsync(cancellationToken);

            return new UpdateWarehouseNameCommandDto
            {
                Id = warehouse.Id,
                Name = warehouse.Name
            };
        }
    }
}
