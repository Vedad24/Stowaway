using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Application.Modules.Storage.Container.Commands.UpdateCanvasPosition
{
    public class UpdateContainerCanvasPositionCommand : IRequest<Unit>
    {
        public int Id { get; set; }
        public double? CanvasX { get; set; }
        public double? CanvasY { get; set; }
    }
}
