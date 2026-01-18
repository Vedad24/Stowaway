using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Application.Modules.Identity.Users.Queries.GetById
{
    public class GetByIdQuery : IRequest<GetByIdQueryDto>
    {
        public int Id { get; set; }
    }
}
