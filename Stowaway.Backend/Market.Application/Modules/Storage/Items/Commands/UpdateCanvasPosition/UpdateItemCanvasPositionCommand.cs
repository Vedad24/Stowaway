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
        /// <summary>Null clears the position, which puts the item back in the unplaced list.</summary>
        public double? CanvasX { get; set; }
        public double? CanvasY { get; set; }
    }
}
