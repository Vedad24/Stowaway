using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Application.Modules.Storage.Items.Commands.Delete
{
    public class DeleteItemCommand : IRequest<Unit>
    {
        public required int Id { get; set; }
    }
}
