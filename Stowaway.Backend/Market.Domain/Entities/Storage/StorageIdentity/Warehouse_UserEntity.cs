using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Market.Domain.Entities.Identity;

namespace Stowaway.Domain.Entities.Storage.StorageIdentity
{
    [Table("Warehouse_User", Schema = "StorageIdentity")]
    public class Warehouse_UserEntity
    {
        public int WarehouseId { get; set; }
        public WarehouseEntity? Warehouse { get; set; }
        public int UserId { get; set; }
        public UserEntity? User { get; set; }
        public int PriviledgeGroupId { get; set; }
        public PriviledgeGroupEntity PriviledgeGroup { get; set; }
    }
}
