using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stowaway.Application.Modules.Storage.Supplier.Queries.GetById;

namespace Stowaway.Application.Modules.Storage.Supplier.Queries.GetById
{
    public sealed class GetSupplierByIdQuery : IRequest<GetSupplierByIdQueryDto>
    {
        public int Id { get; set; }
    }
}
