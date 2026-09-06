namespace Stowaway.Application.Modules.Storage.StorageIdentity.Commands.Delete
{
    public class DeletePriviledgeGroupCommand : IRequest<Unit>
    {
        public required int Id { get; set; }
    }
}
