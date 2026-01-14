// MarketUserEntity.cs
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using Market.Domain.Common;
using Stowaway.Domain.Entities.Identity;

namespace Market.Domain.Entities.Identity;
[Table("User", Schema = "Identity")]

public sealed class UserEntity : BaseEntity
{
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    [ForeignKey(nameof(Role))]
    public Role? RoleId { get; set; }
    public RoleEntity? Role { get; set; }
    public int TokenVersion { get; set; } = 0;// For global revocation
    public bool IsEnabled { get; set; }
    public ICollection<RefreshTokenEntity> RefreshTokens { get; private set; } = new List<RefreshTokenEntity>();
}