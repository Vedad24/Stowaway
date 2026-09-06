using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stowaway.Application.Modules.Storage.Container.Shared;
using Stowaway.Application.Modules.Storage.Items.Shared;
using Stowaway.Application.Modules.Storage.Supplier.Shared;

namespace Stowaway.Application.Modules.Storage.Items.Queries.GetById
{
    public sealed class GetItemByIdQueryDto
    {
        public required int Id { get; init; }
        public required string Name { get; init; }
        public required string Description { get; init; }
        public required byte[] ByteImage { get; init; }
        public required int Quantity { get; init; }
        public required ListContainersDto Container { get; init; }
        public required SharedSupplierDto Supplier { get; init; }
        public required List<SharedTagDto> Tags { get; init; }
        public required List<SharedItemImageDto> Images { get; init; }
    }
}
