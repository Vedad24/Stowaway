using Stowaway.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stowaway.Infrastructure.Database.Configurations.Identity
{
    public class Permission_RoleEntityConfiguration : IEntityTypeConfiguration<Permission_RoleEntity>
    {
        public void Configure(EntityTypeBuilder<Permission_RoleEntity> builder)
        {
            builder.HasKey(pr => new {pr.PermissionId, pr.RoleId});

            builder.HasOne(pr => pr.Permission)
                .WithMany()
                .HasForeignKey(pr => pr.PermissionId)
                .IsRequired();

            builder.HasOne(pr => pr.Role)
                .WithMany()
                .HasForeignKey(pr => pr.RoleId)
                .IsRequired();
        }
    }
}
