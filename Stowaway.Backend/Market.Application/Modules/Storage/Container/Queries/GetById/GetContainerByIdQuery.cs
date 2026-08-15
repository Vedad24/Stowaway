using Stowaway.Application.Modules.Storage.Container.Shared;

namespace Stowaway.Application.Modules.Storage.Container.Queries.GetById
{
    public sealed class GetContainerByIdQuery : IRequest<ListContainersDto>
    {
        public int Id { get; set; }
    }
}
