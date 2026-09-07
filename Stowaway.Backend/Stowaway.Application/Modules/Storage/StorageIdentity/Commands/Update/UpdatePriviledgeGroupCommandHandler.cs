using Stowaway.Domain.Entities.Storage.StorageIdentity;

namespace Stowaway.Application.Modules.Storage.StorageIdentity.Commands.Update
{
    public sealed class UpdatePriviledgeGroupCommandHandler(IAppDbContext ctx)
        : IRequestHandler<UpdatePriviledgeGroupCommand, int>
    {
        public async Task<int> Handle(UpdatePriviledgeGroupCommand request, CancellationToken cancellationToken)
        {
            var priviledgeGroup = await ctx.PriviledgeGroups
                .Include(x => x.Priviledges)
                .FirstOrDefaultAsync(x => x.Id == request.PriviledgeId, cancellationToken);

            if (priviledgeGroup is null)
            {
                throw new StowawayNotFoundException("Privilege group with the supplied id was not found.");
            }

            // The caller's WarehouseUsers.Manage privilege is checked against request.WarehouseId
            // (see WarehouseResolutionStrategy.BodyField on this endpoint's policy) - without this,
            // a manager of warehouse A could name warehouse A in the body while targeting a group
            // id that actually belongs to warehouse B, and edit B's group despite having no rights
            // there. The group's own warehouse is the source of truth and must match.
            if (priviledgeGroup.WarehouseId != request.WarehouseId)
            {
                throw new StowawayBusinessRuleException("priviledge-group.wrong-warehouse", "Privilege group does not belong to the supplied warehouse.");
            }

            var normalizedName = request.Name?.Trim();

            if (string.IsNullOrWhiteSpace(normalizedName))
            {
                throw new ValidationException("Name is required.");
            }

            var warehouseExists = await ctx.Warehouses.AnyAsync(x => x.Id == request.WarehouseId, cancellationToken);
            if (!warehouseExists)
            {
                throw new StowawayNotFoundException("Warehouse with the supplied id was not found.");
            }

            var duplicates = await ctx.PriviledgeGroups
                .AnyAsync(x => x.WarehouseId == request.WarehouseId && x.Name == normalizedName, cancellationToken)
                && normalizedName != priviledgeGroup.Name;

            if (duplicates)
            {
                throw new StowawayConflictException("A privilege group with the same name already exists for this warehouse.");
            }

            var requestedPrivilegeIds = request.PriviledgeIds
                .Where(x => x > 0)
                .Distinct()
                .ToList();

            var validPrivilegeIds = await ctx.Priviledges
                .Where(x => requestedPrivilegeIds.Contains(x.Id))
                .Select(x => x.Id)
                .ToListAsync(cancellationToken);

            if (validPrivilegeIds.Count != requestedPrivilegeIds.Distinct().Count())
            {
                throw new StowawayBusinessRuleException("priviledge-group.invalid-priviledges", "One or more privilege ids are invalid.");
            }

            priviledgeGroup.Name = normalizedName;

            var existingAssociations = await ctx.PriviledgeGroupsPriviledges
                .Where(x => x.PriviledgeGroupId == priviledgeGroup.Id)
                .ToListAsync(cancellationToken);

            if (existingAssociations.Any())
            {
                ctx.PriviledgeGroupsPriviledges.RemoveRange(existingAssociations);
            }

            var newAssociations = validPrivilegeIds
                .Select(privilegeId => new PriviledgeGroup_PriviledgeEntity
                {
                    PriviledgeGroupId = priviledgeGroup.Id,
                    PriviledgeId = privilegeId,
                })
                .ToList();

            if (newAssociations.Any())
            {
                ctx.PriviledgeGroupsPriviledges.AddRange(newAssociations);
            }

            ctx.PriviledgeGroups.Update(priviledgeGroup);
            await ctx.SaveChangesAsync(cancellationToken);

            return priviledgeGroup.Id;
        }
    }
}
