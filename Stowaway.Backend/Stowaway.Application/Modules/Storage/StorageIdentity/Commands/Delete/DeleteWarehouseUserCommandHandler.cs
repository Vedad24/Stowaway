namespace Stowaway.Application.Modules.Storage.StorageIdentity.Commands.Delete
{
    public class DeleteWarehouseUserCommandHandler(IAppDbContext ctx) : IRequestHandler<DeleteWarehouseUserCommand, Unit>
    {
        public async Task<Unit> Handle(DeleteWarehouseUserCommand request, CancellationToken cancellationToken)
        {
            var warehouseUser = await ctx.WarehouseUsers.FirstOrDefaultAsync(
                x => x.WarehouseId == request.WarehouseId
                    && x.UserId == request.UserId
                    && x.PriviledgeGroupId == request.PriviledgeGroupId,
                cancellationToken);

            if (warehouseUser is null)
            {
                throw new StowawayNotFoundException("Warehouse user assignment not found.");
            }

            ctx.WarehouseUsers.Remove(warehouseUser);
            await ctx.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
