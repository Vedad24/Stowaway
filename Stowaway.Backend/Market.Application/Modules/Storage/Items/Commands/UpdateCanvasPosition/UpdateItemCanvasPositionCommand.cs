using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Application.Modules.Storage.Items.Commands.UpdateCanvasPosition
{
    public class UpdateItemCanvasPositionCommand : IRequest<Unit>
    {
        public int Id { get; set; }
        public double? CanvasX { get; set; }
        public double? CanvasY { get; set; }
    }
}
