// MarketUserEntity.cs
using Market.Domain.Common;
using Stowaway.Domain.Entities.Identity;
using System.Data;

namespace Market.Domain.Entities.Identity;

public sealed class UserEntity : BaseEntity
{
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    //public bool IsAdmin { get; set; }
    //public bool IsManager { get; set; }
    //public bool IsEmployee { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public Role? RoleId { get; set; }
    public RoleEntity? Role { get; set; }
    public int TokenVersion { get; set; } = 0;// For global revocation
    public bool IsEnabled { get; set; }
    public ICollection<RefreshTokenEntity> RefreshTokens { get; private set; } = new List<RefreshTokenEntity>();
}