using Stowaway.Shared.Constants;
using Stowaway.Domain.Entities.Storage.StorageIdentity;

namespace Stowaway.Application.Modules.Storage.StorageIdentity
{
    public static class WarehouseOwnerProvisioning
    {
        public const string OwnerGroupName = "Owner";

        public static async Task EnsureOwnerAccessAsync(IAppDbContext ctx, int warehouseId, int userId, CancellationToken cancellationToken)
        {
            var ownerGroup = new PriviledgeGroupEntity
            {
                Name = OwnerGroupName,
                WarehouseId = warehouseId,
            };

            ctx.PriviledgeGroups.Add(ownerGroup);
            await ctx.SaveChangesAsync(cancellationToken);

            var privilegeIds = await ctx.Priviledges
                .Where(p => Stowaway.Shared.Constants.Priviledges.AllCodes.Contains(p.Code))
                .Select(p => p.Id)
                .ToListAsync(cancellationToken);

            foreach (var privilegeId in privilegeIds)
            {
                ctx.PriviledgeGroupsPriviledges.Add(new PriviledgeGroup_PriviledgeEntity
                {
                    PriviledgeGroupId = ownerGroup.Id,
                    PriviledgeId = privilegeId,
                });
            }

            ctx.WarehouseUsers.Add(new Warehouse_UserEntity
            {
                WarehouseId = warehouseId,
                UserId = userId,
                PriviledgeGroupId = ownerGroup.Id,
            });

            await ctx.SaveChangesAsync(cancellationToken);
        }
    }
}
