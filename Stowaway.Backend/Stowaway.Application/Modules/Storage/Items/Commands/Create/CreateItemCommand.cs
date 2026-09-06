using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stowaway.Application.Modules.Storage.Supplier.Shared;

namespace Stowaway.Application.Modules.Storage.Items.Commands.Create
{
    public class CreateItemCommand : IRequest<int>
    {
        public string Name { get; init; }
        public string Description { get; init; }
        public byte[]? ByteImage { get; init; }
        public int Quantity { get; init; }
        public int ContainerId { get; init; }
        public int SupplierId { get; init; }
        public List<int> TagIds { get; init; } = new();
        public List<string> Images { get; init; } = new();
    }
}
