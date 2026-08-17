namespace Stowaway.Application.Modules.Storage.Container.Commands.Update
{
    public sealed class UpdateContainerCommand : IRequest<Unit>
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int ContainerTypeId { get; set; }
    }
}