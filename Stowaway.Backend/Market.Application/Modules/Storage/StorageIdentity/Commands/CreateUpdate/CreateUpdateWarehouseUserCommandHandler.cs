using Stowaway.Domain.Entities.Storage.StorageIdentity;

namespace Stowaway.Application.Modules.Storage.StorageIdentity.Commands.CreateUpdate
{
    public sealed class CreateUpdateWarehouseUserCommandHandler(IAppDbContext ctx)
        : IRequestHandler<CreateUpdateWarehouseUserCommand, CreateUpdateWarehouseUserCommandDto>
    {
        public async Task<CreateUpdateWarehouseUserCommandDto> Handle(CreateUpdateWarehouseUserCommand request, CancellationToken cancellationToken)
        {
            var userExists = await ctx.Users.AnyAsync(u => u.Id == request.UserId, cancellationToken);
            if (!userExists)
            {
                throw new Exception("User with the supplied id was not found.");
            }

            var warehouseExists = await ctx.Warehouses.AnyAsync(w => w.Id == request.WarehouseId, cancellationToken);
            if (!warehouseExists)
            {
                throw new Exception("Warehouse with the supplied id was not found.");
            }

            var priviledgeGroup = await ctx.PriviledgeGroups
                .FirstOrDefaultAsync(g => g.Id == request.PriviledgeGroupId, cancellationToken);

            if (priviledgeGroup is null)
            {
                throw new Exception("Privilege group with the supplied id was not found.");
            }

            if (priviledgeGroup.WarehouseId != request.WarehouseId)
            {
                throw new Exception("Privilege group does not belong to the supplied warehouse.");
            }

            var existingAssignment = await ctx.WarehouseUsers
                .FirstOrDefaultAsync(wu => wu.WarehouseId == request.WarehouseId && wu.UserId == request.UserId, cancellationToken);

            if (existingAssignment is not null)
            {
                if (existingAssignment.PriviledgeGroupId != request.PriviledgeGroupId)
                {
                    ctx.WarehouseUsers.Remove(existingAssignment);
                    ctx.WarehouseUsers.Add(new Warehouse_UserEntity
                    {
                        WarehouseId = request.WarehouseId,
                        UserId = request.UserId,
                        PriviledgeGroupId = request.PriviledgeGroupId,
                    });
                }
            }
            else
            {
                ctx.WarehouseUsers.Add(new Warehouse_UserEntity
                {
                    WarehouseId = request.WarehouseId,
                    UserId = request.UserId,
                    PriviledgeGroupId = request.PriviledgeGroupId,
                });
            }

            await ctx.SaveChangesAsync(cancellationToken);

            return new CreateUpdateWarehouseUserCommandDto
            {
                UserId = request.UserId,
                WarehouseId = request.WarehouseId,
                PriviledgeGroupId = request.PriviledgeGroupId,
            };
        }
    }
}
