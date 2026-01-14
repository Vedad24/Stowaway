using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Application.Modules.Identity.Commands.Delete
{
    public class DeleteUserCommandHandler(IAppDbContext context) : IRequestHandler<DeleteUserCommand, bool>
    {
        public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = context.Users.FirstOrDefault(u => u.Id == request.Id);
            if(user is null)
                throw new StowawayNotFoundException($"User not found -> Id: {request.Id}");
            context.Users.Remove(user);
            await context.SaveChangesAsync(cancellationToken);
            return true;

        }
    }
}
