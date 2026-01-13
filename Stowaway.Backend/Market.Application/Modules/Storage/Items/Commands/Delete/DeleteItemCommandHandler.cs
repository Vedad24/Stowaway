using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Application.Modules.Storage.Items.Commands.Delete
{
    public class DeleteItemCommandHandler(IAppDbContext ctx) : IRequestHandler<DeleteItemCommand, Unit>
    {
        public async Task<Unit> Handle(DeleteItemCommand request, CancellationToken cancellationToken)
        {
            var item = await ctx.Item.FirstOrDefaultAsync( x => x.Id == request.Id, cancellationToken);

            if (item == null)
            {
                throw new Exception($"Item with id {request.Id} is not found");
            }

            ctx.Item.Remove(item);
            await ctx.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
