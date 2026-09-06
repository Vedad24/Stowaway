using Stowaway.Shared.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Stowaway.API.Authorization;

public static class JwtOptionsExtensions
{
    public static TokenValidationParameters ToTokenValidationParameters(this JwtOptions jwt) => new()
    {
        ValidateIssuer = true,
        ValidIssuer = jwt.Issuer,
        ValidateAudience = true,
        ValidAudience = jwt.Audience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
}
