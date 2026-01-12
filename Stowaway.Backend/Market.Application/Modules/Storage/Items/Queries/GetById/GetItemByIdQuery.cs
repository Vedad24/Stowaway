using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Application.Modules.Storage.Items.Queries.GetById
{
    public sealed class GetItemByIdQuery : IRequest<GetItemByIdQueryDto>
    {
        public int Id { get; set; }
    }   
}
