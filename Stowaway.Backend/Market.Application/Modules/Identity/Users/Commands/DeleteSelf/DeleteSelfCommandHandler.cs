namespace Stowaway.Application.Modules.Identity.Users.Commands.DeleteSelf
{
    public class DeleteSelfCommandHandler(IAppDbContext context, IAppCurrentUser currentUser)
        : IRequestHandler<DeleteSelfCommand, bool>
    {
        public async Task<bool> Handle(DeleteSelfCommand request, CancellationToken cancellationToken)
        {
            if (currentUser.UserId is null)
                throw new StowawayNotFoundException("Current user could not be resolved.");

            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == currentUser.UserId, cancellationToken);
            if (user is null)
                throw new StowawayNotFoundException($"User with ID : {currentUser.UserId} not found");

            context.Users.Remove(user);
            await context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
