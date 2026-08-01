using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Application.Modules.Storage.Items.Commands.Move
{
    public class MoveItemCommand : IRequest<Unit>
    {
        public int Id { get; set; }
        public int ContainerId { get; set; }
    }
}
