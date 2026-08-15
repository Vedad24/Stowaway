using Stowaway.Application.Modules.Storage.Container.Shared;
using Stowaway.Domain.Entities.Storage;

namespace Stowaway.Application.Modules.Storage.Container.Queries.GetById
{
    public class GetContainerByIdQueryHandler(IAppDbContext ctx) : IRequestHandler<GetContainerByIdQuery, ListContainersDto>
    {
        public async Task<ListContainersDto> Handle(GetContainerByIdQuery request, CancellationToken cancellationToken)
        {
            var container = await ctx.Containers
                .Where(x => x.Id == request.Id)
                .Select(x => new ListContainersDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    ContainerTypeId = x.ContainerTypeId,
                    WarehouseId = x.WarehouseId,
                    WarehouseName = x.Warehouse!.Name,
                    ParentContainerId = x.ParentContainerId,
                    HasChildren = ctx.Containers.Any(child => child.ParentContainerId == x.Id),
                    CanvasX = x.CanvasX,
                    CanvasY = x.CanvasY,
                    MaxItems = x.ContainerType!.MaxItems,
                    MaxContainers = x.ContainerType!.MaxContainers,
                    ContainerCountUsed = ctx.Containers.Count(child => child.ParentContainerId == x.Id),
                    CurrentStatus = ctx.ContainerStatusHistories
                        .Where(h => h.ContainerId == x.Id)
                        .OrderByDescending(h => h.Date)
                        .Select(h => new SharedContainerStatusDto { Id = h.Status.Id, Name = h.Status.Description })
                        .FirstOrDefault(),
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (container is null)
            {
                throw new Exception($"Container with id: {request.Id} not found");
            }

            container.ItemQuantityUsed = await ContainerCapacityHelper.GetRecursiveItemQuantity(ctx, container.Id, cancellationToken);
            container.ContainerTypeName = ContainerTypeEntity.DescribeSize(container.MaxItems, container.MaxContainers);

            return container;
        }
    }
}
