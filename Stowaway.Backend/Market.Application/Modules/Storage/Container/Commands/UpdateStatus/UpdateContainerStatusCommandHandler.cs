using Stowaway.Domain.Entities.Storage;

namespace Stowaway.Application.Modules.Storage.Container.Commands.UpdateStatus
{
    public class UpdateContainerStatusCommandHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
        : IRequestHandler<UpdateContainerStatusCommand, Unit>
    {
        private static readonly string[] AllowedStatuses = ["Incoming", "Outgoing"];

        public async Task<Unit> Handle(UpdateContainerStatusCommand request, CancellationToken cancellationToken)
        {
            if (!AllowedStatuses.Contains(request.Status, StringComparer.OrdinalIgnoreCase))
            {
                throw new ValidationException($"Status must be one of: {string.Join(", ", AllowedStatuses)}.");
            }

            var container = await ctx.Containers.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (container is null)
            {
                throw new Exception($"Container with id: {request.Id} not found");
            }

            if (currentUser.UserId is null)
            {
                throw new Exception("Current user could not be resolved.");
            }

            var status = await ctx.ContainerStatuses.FirstOrDefaultAsync(
                s => s.Description == request.Status, cancellationToken);

            if (status is null)
            {
                status = new ContainerStatusEntity { Description = request.Status };
                ctx.ContainerStatuses.Add(status);
                await ctx.SaveChangesAsync(cancellationToken);
            }

            ctx.ContainerStatusHistories.Add(new ContainerStatusHistoryEntity
            {
                ContainerId = container.Id,
                StatusId = status.Id,
                UserId = currentUser.UserId.Value,
                Date = DateTime.UtcNow,
            });

            await ctx.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
