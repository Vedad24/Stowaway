using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Application.Modules.Storage.Items.Commands.Update
{
    public class UpdateItemCommand : IRequest<Unit>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public byte[]? ByteImage { get; set; }
        public int Quantity { get; set; }
        public int ContainerId { get; set; }
        public int SupplierId { get; set; }
        public List<int> TagIds { get; set; } = new();
    }
}
