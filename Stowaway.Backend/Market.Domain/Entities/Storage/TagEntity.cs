using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Domain.Entities.Storage
{
    [Table("Tag", Schema = "Storage")]

    public class TagEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
