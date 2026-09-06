using Microsoft.AspNetCore.Antiforgery;

namespace Stowaway.API.Authorization;

public static class AuthCookies
{
    public const string AccessTokenCookieName = "access_token";
    public const string RefreshTokenCookieName = "refresh_token";
    public const string XsrfTokenCookieName = "XSRF-TOKEN";

    public static void SetAccessToken(this HttpResponse response, IHostEnvironment env, string token, DateTime expiresAtUtc)
    {
        response.Cookies.Append(AccessTokenCookieName, token, BuildOptions(env, expiresAtUtc, path: "/", httpOnly: true));
    }

    public static void SetRefreshToken(this HttpResponse response, IHostEnvironment env, string token, DateTime expiresAtUtc)
    {
        response.Cookies.Append(RefreshTokenCookieName, token, BuildOptions(env, expiresAtUtc, path: "/api/auth", httpOnly: true));
    }

    // Readable by JS on purpose - this is the antiforgery RequestToken (AntiforgeryTokenSet.RequestToken),
    // not a secret. Angular's XSRF interceptor reads it and echoes it back as the X-XSRF-TOKEN header.
    public static void SetXsrfToken(this HttpResponse response, IHostEnvironment env, string requestToken, DateTime expiresAtUtc)
    {
        response.Cookies.Append(XsrfTokenCookieName, requestToken, BuildOptions(env, expiresAtUtc, path: "/", httpOnly: false));
    }

    // Mints a fresh antiforgery pair and issues the readable half as the XSRF-TOKEN cookie.
    // Must only be called from an already-authenticated request (e.g. GET /User/me) - the
    // antiforgery system embeds the caller's identity in the token and rejects it later if
    // that identity doesn't match the request that validates it (e.g. minting during an
    // anonymous-at-call-time Login response, then validating on an authenticated Logout).
    public static void IssueXsrfToken(this HttpContext httpContext, IAntiforgery antiforgery, IHostEnvironment env, DateTime expiresAtUtc)
    {
        var afTokens = antiforgery.GetAndStoreTokens(httpContext);
        httpContext.Response.SetXsrfToken(env, afTokens.RequestToken!, expiresAtUtc);
    }

    public static void ClearAuthCookies(this HttpResponse response, IHostEnvironment env)
    {
        response.Cookies.Delete(AccessTokenCookieName, BuildOptions(env, DateTime.UtcNow, path: "/", httpOnly: true));
        response.Cookies.Delete(RefreshTokenCookieName, BuildOptions(env, DateTime.UtcNow, path: "/api/auth", httpOnly: true));
        response.Cookies.Delete(XsrfTokenCookieName, BuildOptions(env, DateTime.UtcNow, path: "/", httpOnly: false));
    }

    private static CookieOptions BuildOptions(IHostEnvironment env, DateTime expiresAtUtc, string path, bool httpOnly) => new()
    {
        HttpOnly = httpOnly,
        Secure = !env.IsDevelopment(),
        SameSite = SameSiteMode.Lax,
        Path = path,
        Expires = expiresAtUtc
    };
}
