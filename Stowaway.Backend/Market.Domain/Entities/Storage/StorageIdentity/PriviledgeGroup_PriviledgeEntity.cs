using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Domain.Entities.Storage.StorageIdentity
{
    [Table("PriviledgeGroup_Priviledge", Schema = "StorageIdentity")]
    public class PriviledgeGroup_PriviledgeEntity
    {
        public int PriviledgeGroupId { get; set; }
        public PriviledgeGroupEntity PriviledgeGroup { get; set; }
        public int PriviledgeId {  set; get; } 
        public PriviledgeEntity Priviledge { set; get; }
    }
}
