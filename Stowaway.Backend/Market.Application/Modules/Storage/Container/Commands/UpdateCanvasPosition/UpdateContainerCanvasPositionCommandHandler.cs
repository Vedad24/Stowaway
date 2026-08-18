using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Application.Modules.Storage.Container.Commands.UpdateCanvasPosition
{
    public class UpdateContainerCanvasPositionCommandHandler(IAppDbContext ctx)
        : IRequestHandler<UpdateContainerCanvasPositionCommand, Unit>
    {
        public async Task<Unit> Handle(UpdateContainerCanvasPositionCommand request, CancellationToken cancellationToken)
        {
            var container = await ctx.Containers.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (container is null)
            {
                throw new StowawayNotFoundException($"Container with id: {request.Id} not found");
            }

            if (request.CanvasX.HasValue != request.CanvasY.HasValue)
            {
                throw new StowawayBusinessRuleException("canvas-position.incomplete", "Canvas position needs both X and Y, or neither");
            }

            container.CanvasX = request.CanvasX;
            container.CanvasY = request.CanvasY;

            await ctx.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
