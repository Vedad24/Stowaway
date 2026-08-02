namespace Stowaway.Application.Modules.Storage.Container.Commands.Update
{
    public class UpdateContainerCommandHandler(IAppDbContext ctx) : IRequestHandler<UpdateContainerCommand, Unit>
    {
        public async Task<Unit> Handle(UpdateContainerCommand request, CancellationToken cancellationToken)
        {
            var normalized = request.Name?.Trim();

            if (string.IsNullOrWhiteSpace(normalized))
            {
                throw new ValidationException("Name is required.");
            }

            var container = await ctx.Containers.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (container is null)
            {
                throw new Exception($"Container with id: {request.Id} not found");
            }

            var containerType = await ctx.ContainerTypes.FirstOrDefaultAsync(x => x.Id == request.ContainerTypeId, cancellationToken);
            if (containerType is null)
            {
                throw new Exception($"Container type with id: {request.ContainerTypeId} not found");
            }

            container.Name = normalized;
            container.ContainerTypeId = request.ContainerTypeId;

            await ctx.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
