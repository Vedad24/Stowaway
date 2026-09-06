using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Application.Modules.Sales.Order.Queries.GetById
{
    public class GetOrderByIdQuery : IRequest<GetOrderByIdQueryDto>
    {
        public required int Id { get; set; }
    }
}
