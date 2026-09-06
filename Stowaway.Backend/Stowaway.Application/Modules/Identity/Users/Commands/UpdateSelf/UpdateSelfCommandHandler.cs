namespace Stowaway.Application.Modules.Identity.Users.Commands.UpdateSelf
{
    public class UpdateSelfCommandHandler(IAppDbContext context, IAppCurrentUser currentUser)
        : IRequestHandler<UpdateSelfCommand, UpdateSelfCommandDto>
    {
        public async Task<UpdateSelfCommandDto> Handle(UpdateSelfCommand request, CancellationToken cancellationToken)
        {
            if (currentUser.UserId is null)
                throw new StowawayNotFoundException("Current user could not be resolved.");

            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == currentUser.UserId, cancellationToken);
            if (user is null)
                throw new StowawayNotFoundException($"User with ID : {currentUser.UserId} not found");

            user.Email = request.Email ?? user.Email;
            user.FirstName = request.FirstName ?? user.FirstName;
            user.LastName = request.LastName ?? user.LastName;

            await context.SaveChangesAsync(cancellationToken);

            return new UpdateSelfCommandDto
            {
                Email = user.Email,
                FirstName = user.FirstName ?? "[no_name]",
                LastName = user.LastName ?? "[no_name]"
            };
        }
    }
}
