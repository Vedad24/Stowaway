using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Domain.Entities.Storage.StorageIdentity
{
    [Table("Priviledge", Schema = "StorageIdentity")]
    public class PriviledgeEntity
    {
        public int Id { get; set; }
        public string Description { get; set; }
    }
}
