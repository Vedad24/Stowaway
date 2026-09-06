namespace Stowaway.Application.Modules.Storage.StorageIdentity.Commands.Delete
{
    public class DeletePriviledgeGroupCommandHandler(IAppDbContext ctx) : IRequestHandler<DeletePriviledgeGroupCommand, Unit>
    {
        public async Task<Unit> Handle(DeletePriviledgeGroupCommand request, CancellationToken cancellationToken)
        {
            var priviledgeGroup = await ctx.PriviledgeGroups.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (priviledgeGroup is null)
            {
                throw new StowawayNotFoundException($"Priviledge group with id: {request.Id} not found");
            }

            var hasWarehouseUsers = await ctx.WarehouseUsers.AnyAsync(x => x.PriviledgeGroupId == priviledgeGroup.Id, cancellationToken);
            if (hasWarehouseUsers)
            {
                throw new ValidationException("Priviledge group is assigned to warehouse users. Reassign those users before deleting the group.");
            }

            var groupPriviledges = await ctx.PriviledgeGroupsPriviledges
                .Where(x => x.PriviledgeGroupId == priviledgeGroup.Id)
                .ToListAsync(cancellationToken);
            ctx.PriviledgeGroupsPriviledges.RemoveRange(groupPriviledges);

            ctx.PriviledgeGroups.Remove(priviledgeGroup);
            await ctx.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
