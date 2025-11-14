using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Domain.Entities.Identity
{
    public enum Permission
    {
        CreateAccount = 1,
        DeleteOwnAccount,
        DeleteAccount,
        CreateWarehouse,
        DeleteOwnWarehouse,
        DeleteWarehouse,
        ChangeWarehouseConfiguration,
        ChageWarehousePriviledges,
        //Add More
    }
    [Table("Permission", Schema = "Identity")]

    public class PermissionEntity
    {
        public Permission Id { get; set; }
        public string Description { get; set; }
    }
}
