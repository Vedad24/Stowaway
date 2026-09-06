using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Application.Modules.Storage.Items.Commands.SetFavourite
{
    public class SetItemFavouriteCommand : IRequest<Unit>
    {
        public int Id { get; set; }
        public bool IsFavourite { get; set; }
    }
}
