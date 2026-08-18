using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Application.Modules.Storage.Items.Commands.UpdateCanvasPosition
{
    public class UpdateItemCanvasPositionCommandHandler(IAppDbContext ctx)
        : IRequestHandler<UpdateItemCanvasPositionCommand, Unit>
    {
        public async Task<Unit> Handle(UpdateItemCanvasPositionCommand request, CancellationToken cancellationToken)
        {
            var item = await ctx.Item.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (item is null)
            {
                throw new StowawayNotFoundException($"Item with id: {request.Id} not found");
            }

            if (request.CanvasX.HasValue != request.CanvasY.HasValue)
            {
                throw new StowawayBusinessRuleException("canvas-position.incomplete", "Canvas position needs both X and Y, or neither");
            }

            item.CanvasX = request.CanvasX;
            item.CanvasY = request.CanvasY;

            await ctx.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
