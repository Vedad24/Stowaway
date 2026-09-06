using Stowaway.Application.Modules.Storage.Items.Shared;

namespace Stowaway.Application.Modules.Storage.Tags.Commands.Create
{
    public sealed class CreateTagCommand : IRequest<SharedTagDto>
    {
        public required string Name { get; init; }
    }
}
