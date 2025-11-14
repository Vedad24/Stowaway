using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Market.Domain.Entities.Identity;

namespace Stowaway.Domain.Entities.Storage
{
    [Table("ContainerStatusHistory", Schema = "Storage")]
    public class ContainerStatusHistoryEntity
    {
        public int Id { get; set; }
        public int ContainerId { get; set; }
        public ContainerEntity Container { get; set; }
        public int StatusId { get; set; }
        public ContainerStatusEntity Status {  get; set; }
        public int UserId { get; set; }
        public UserEntity? User { get; set; }
        public DateTime Date {  get; set; }
    }
}
