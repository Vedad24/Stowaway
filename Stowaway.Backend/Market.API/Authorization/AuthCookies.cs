namespace Market.API.Authorization;

public static class AuthCookies
{
    public const string AccessTokenCookieName = "access_token";
    public const string RefreshTokenCookieName = "refresh_token";

    public static void SetAccessToken(this HttpResponse response, IHostEnvironment env, string token, DateTime expiresAtUtc)
    {
        response.Cookies.Append(AccessTokenCookieName, token, BuildOptions(env, expiresAtUtc, path: "/"));
    }

    public static void SetRefreshToken(this HttpResponse response, IHostEnvironment env, string token, DateTime expiresAtUtc)
    {
        response.Cookies.Append(RefreshTokenCookieName, token, BuildOptions(env, expiresAtUtc, path: "/api/auth"));
    }

    public static void ClearAuthCookies(this HttpResponse response, IHostEnvironment env)
    {
        response.Cookies.Delete(AccessTokenCookieName, BuildOptions(env, DateTime.UtcNow, path: "/"));
        response.Cookies.Delete(RefreshTokenCookieName, BuildOptions(env, DateTime.UtcNow, path: "/api/auth"));
    }

    private static CookieOptions BuildOptions(IHostEnvironment env, DateTime expiresAtUtc, string path) => new()
    {
        HttpOnly = true,
        Secure = !env.IsDevelopment(),
        SameSite = SameSiteMode.Lax,
        Path = path,
        Expires = expiresAtUtc
    };
}
