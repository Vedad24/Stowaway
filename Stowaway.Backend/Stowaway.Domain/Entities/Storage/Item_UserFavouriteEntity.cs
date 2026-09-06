using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stowaway.Domain.Entities.Identity;

namespace Stowaway.Domain.Entities.Storage
{
    [Table("Item_UserFavourite", Schema = "Storage")]

    public class Item_UserFavouriteEntity
    {
        public int ItemId { get; set; }
        public ItemEntity? Item { get; set; }

        public int UserId { get; set; }
        public UserEntity? User { get; set; }

    }
}
