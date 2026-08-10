namespace Stowaway.Application.Modules.Storage.Container.Commands.Delete
{
    public class DeleteContainerCommand : IRequest<Unit>
    {
        public int Id { get; set; }
        public bool DeleteContents { get; set; }
        public int? MoveContentsToContainerId { get; set; }
    }
}
