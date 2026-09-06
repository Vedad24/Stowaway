using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Application.Modules.Storage.Items.Shared
{
    public sealed class SharedTagDto
    {
        public required int Id { get; init; }
        public required string Name { get; init; }
    }
}
