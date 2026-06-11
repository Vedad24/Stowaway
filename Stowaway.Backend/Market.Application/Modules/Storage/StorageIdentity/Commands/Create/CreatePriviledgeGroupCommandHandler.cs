using Stowaway.Domain.Entities.Storage.StorageIdentity;

namespace Stowaway.Application.Modules.Storage.StorageIdentity.Commands.Create
{
    public sealed class CreatePriviledgeGroupCommandHandler(IAppDbContext ctx)
        : IRequestHandler<CreatePriviledgeGroupCommand, int>
    {
        public async Task<int> Handle(CreatePriviledgeGroupCommand request, CancellationToken cancellationToken)
        {
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
                .AnyAsync(x => x.WarehouseId == request.WarehouseId && x.Name == normalizedName, cancellationToken);

            if (duplicates)
            {
                throw new Exception("A privilege group with the same name already exists for this warehouse.");
            }

            var validPrivilegeIds = await ctx.Priviledges
                .Where(x => request.PriviledgeIds.Contains(x.Id))
                .Select(x => x.Id)
                .ToListAsync(cancellationToken);

            if (validPrivilegeIds.Count != request.PriviledgeIds.Distinct().Count())
            {
                throw new Exception("One or more privilege ids are invalid.");
            }

            var group = new PriviledgeGroupEntity
            {
                Name = normalizedName,
                WarehouseId = request.WarehouseId,
                Priviledges = new List<PriviledgeGroup_PriviledgeEntity>()
            };

            ctx.PriviledgeGroups.Add(group);
            await ctx.SaveChangesAsync(cancellationToken);

            foreach (var privilegeId in validPrivilegeIds)
            {
                ctx.PriviledgeGroupsPriviledges.Add(new PriviledgeGroup_PriviledgeEntity
                {
                    PriviledgeGroupId = group.Id,
                    PriviledgeId = privilegeId,
                });
            }

            await ctx.SaveChangesAsync(cancellationToken);

            return group.Id;
        }
    }
}
