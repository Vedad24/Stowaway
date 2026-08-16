using Stowaway.Application.Modules.Storage.Items.Shared;
using Stowaway.Domain.Entities.Storage;

namespace Stowaway.Application.Modules.Storage.Tags.Commands.Create
{
    public class CreateTagCommandHandler(IAppDbContext ctx) : IRequestHandler<CreateTagCommand, SharedTagDto>
    {
        public async Task<SharedTagDto> Handle(CreateTagCommand request, CancellationToken cancellationToken)
        {
            var normalized = request.Name?.Trim();

            if (string.IsNullOrWhiteSpace(normalized))
            {
                throw new ValidationException("Tag name is required.");
            }

            var existing = await ctx.Tags.FirstOrDefaultAsync(
                t => t.Name.ToLower() == normalized.ToLower(), cancellationToken);

            if (existing is not null)
            {
                return new SharedTagDto { Id = existing.Id, Name = existing.Name };
            }

            var tag = new TagEntity { Name = normalized };
            ctx.Tags.Add(tag);
            await ctx.SaveChangesAsync(cancellationToken);

            return new SharedTagDto { Id = tag.Id, Name = tag.Name };
        }
    }
}
