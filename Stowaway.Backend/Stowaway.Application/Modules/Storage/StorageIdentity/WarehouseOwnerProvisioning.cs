using Stowaway.Shared.Constants;
using Stowaway.Domain.Entities.Storage;
using Stowaway.Domain.Entities.Storage.StorageIdentity;

namespace Stowaway.Application.Modules.Storage.StorageIdentity
{
    public static class WarehouseOwnerProvisioning
    {
        public const string OwnerGroupName = "Owner";

        // Stages entities via navigation properties only — does not call SaveChangesAsync,
        // so the caller can add `warehouse` and this method's entities in one graph and save once.
        public static async Task EnsureOwnerAccessAsync(IAppDbContext ctx, WarehouseEntity warehouse, int userId, CancellationToken cancellationToken)
        {
            var privilegeIds = await ctx.Priviledges
                .Where(p => Stowaway.Shared.Constants.Priviledges.AllCodes.Contains(p.Code))
                .Select(p => p.Id)
                .ToListAsync(cancellationToken);

            var ownerGroup = new PriviledgeGroupEntity
            {
                Name = OwnerGroupName,
                Warehouse = warehouse,
                Priviledges = privilegeIds
                    .Select(privilegeId => new PriviledgeGroup_PriviledgeEntity { PriviledgeId = privilegeId })
                    .ToList(),
            };

            ctx.PriviledgeGroups.Add(ownerGroup);

            ctx.WarehouseUsers.Add(new Warehouse_UserEntity
            {
                Warehouse = warehouse,
                UserId = userId,
                PriviledgeGroup = ownerGroup,
            });
        }
    }
}
