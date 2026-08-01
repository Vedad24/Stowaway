using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Application.Modules.Storage.Container.Commands.Move
{
    public class MoveContainerCommand : IRequest<Unit>
    {
        public int Id { get; set; }
        public int? ParentContainerId { get; set; }
    }
}
