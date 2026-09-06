using Stowaway.API.Authorization;
using Stowaway.Application.Modules.Auth.Commands.Login;
using Stowaway.Application.Modules.Auth.Commands.Logout;
using Stowaway.Application.Modules.Auth.Commands.Refresh;
using Stowaway.Shared.Options;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(IMediator mediator, IAntiforgery antiforgery, IHostEnvironment env, IOptions<JwtOptions> jwtOptions) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken ct)
    {
        var tokens = await mediator.Send(command, ct);

        Response.SetAccessToken(env, tokens.AccessToken, tokens.AccessTokenExpiresAtUtc);
        Response.SetRefreshToken(env, tokens.RefreshToken, tokens.RefreshTokenExpiresAtUtc);
        IssueXsrfTokenFor(tokens.AccessToken, tokens.RefreshTokenExpiresAtUtc);

        return Ok();
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh(CancellationToken ct)
    {
        if (!Request.Cookies.TryGetValue(AuthCookies.RefreshTokenCookieName, out var refreshToken) || string.IsNullOrEmpty(refreshToken))
            return Unauthorized();

        // No antiforgery check here: this endpoint exists specifically to run with an expired/absent
        // access token, i.e. an anonymous request - identity-bound antiforgery validation would
        // reject that by design, defeating the endpoint's purpose. CSRF protection here comes from
        // the cookies' SameSite=Lax attribute, which already blocks cross-site POSTs (fetch/XHR and
        // form-based) from attaching them in modern browsers.
        var tokens = await mediator.Send(new RefreshTokenCommand { RefreshToken = refreshToken }, ct);

        Response.SetAccessToken(env, tokens.AccessToken, tokens.AccessTokenExpiresAtUtc);
        Response.SetRefreshToken(env, tokens.RefreshToken, tokens.RefreshTokenExpiresAtUtc);
        IssueXsrfTokenFor(tokens.AccessToken, tokens.RefreshTokenExpiresAtUtc);

        return Ok();
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        if (!await TryValidateAntiforgeryAsync())
            return Forbid();

        if (Request.Cookies.TryGetValue(AuthCookies.RefreshTokenCookieName, out var refreshToken) && !string.IsNullOrEmpty(refreshToken))
            await mediator.Send(new LogoutCommand { RefreshToken = refreshToken }, ct);

        Response.ClearAuthCookies(env);

        return Ok();
    }

    private async Task<bool> TryValidateAntiforgeryAsync()
    {
        try
        {
            await antiforgery.ValidateRequestAsync(HttpContext);
            return true;
        }
        catch (AntiforgeryValidationException ex)
        {
            //Console.WriteLine($"[DEBUG ANTIFORGERY] -> {ex.Message}");
            return false;
        }
    }

    // Login/Refresh are [AllowAnonymous], so HttpContext.User is still anonymous when this runs -
    // setting a Set-Cookie header for access_token doesn't retroactively authenticate THIS request.
    // Antiforgery embeds the caller's identity in the minted token, so minting while anonymous would
    // make it fail validation later against an authenticated request (e.g. Logout). To mint a token
    // bound to the real identity right now, we validate the access token we just issued through the
    // exact same pipeline the JWT-bearer middleware uses on future requests, guaranteeing an
    // identical ClaimsPrincipal - then mint against that.
    private void IssueXsrfTokenFor(string accessToken, DateTime expiresAtUtc)
    {
        var principal = new JwtSecurityTokenHandler().ValidateToken(accessToken, jwtOptions.Value.ToTokenValidationParameters(), out _);
        HttpContext.User = principal;

        HttpContext.IssueXsrfToken(antiforgery, env, expiresAtUtc);
    }
}
