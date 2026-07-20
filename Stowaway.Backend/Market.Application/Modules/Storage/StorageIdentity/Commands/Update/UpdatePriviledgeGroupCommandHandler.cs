using Stowaway.Domain.Entities.Storage.StorageIdentity;

namespace Stowaway.Application.Modules.Storage.StorageIdentity.Commands.Update
{
    public sealed class UpdatePriviledgeGroupCommandHandler(IAppDbContext ctx)
        : IRequestHandler<UpdatePriviledgeGroupCommand, int>
    {
        public async Task<int> Handle(UpdatePriviledgeGroupCommand request, CancellationToken cancellationToken)
        {
            bool priviledgeGroupExists = await ctx.PriviledgeGroups
                .AnyAsync(x => x.Id == request.PriviledgeId, cancellationToken);
            
            if(!priviledgeGroupExists)
            {
                throw new Exception("Privilege group with the supplied id was not found.");
            }

            var priviledgeGroup = await ctx.PriviledgeGroups
                .Include(x => x.Priviledges)
                .FirstOrDefaultAsync(x => x.Id == request.PriviledgeId, cancellationToken);

            var normalizedName = request.Name?.Trim();

            if (string.IsNullOrWhiteSpace(normalizedName))
            {
                throw new ValidationException("Name is required.");
            }

            var warehouseExists = await ctx.Warehouses.AnyAsync(x => x.Id == request.WarehouseId, cancellationToken);
            if (!warehouseExists)
            {
                throw new Exception("Warehouse with the supplied id was not found.");
            }

            var duplicates = await ctx.PriviledgeGroups
                .AnyAsync(x => x.WarehouseId == request.WarehouseId && x.Name == normalizedName, cancellationToken)
                && normalizedName != priviledgeGroup?.Name;

            if (duplicates)
            {
                throw new Exception("A privilege group with the same name already exists for this warehouse.");
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
                throw new Exception("One or more privilege ids are invalid.");
            }

            priviledgeGroup!.Name = normalizedName;
            priviledgeGroup!.WarehouseId = request.WarehouseId;

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
