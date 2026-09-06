using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Application.Modules.Identity.Users.Queries.GetByMail
{
    public class GetByMailQuery : IRequest<GetByMailQueryDto>
    {
        public string Mail { get; set; }
    }
}
