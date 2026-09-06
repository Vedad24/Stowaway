namespace Stowaway.Application.Modules.Storage.Container.Commands.UpdateStatus
{
    public sealed class UpdateContainerStatusCommand : IRequest<Unit>
    {
        public int Id { get; set; }
        public required string Status { get; set; }
    }
}
