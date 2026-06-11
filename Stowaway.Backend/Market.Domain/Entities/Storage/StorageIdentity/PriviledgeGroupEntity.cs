using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Domain.Entities.Storage.StorageIdentity
{
    [Table("PriviledgeGroup", Schema = "StorageIdentity")]
    public class PriviledgeGroupEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int WarehouseId { get; set; }
        public WarehouseEntity? Warehouse { get; set; }
        public List<PriviledgeGroup_PriviledgeEntity>? Priviledges { get; set; }
    }
}
