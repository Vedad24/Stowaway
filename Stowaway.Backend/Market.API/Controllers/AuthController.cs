using Market.API.Authorization;
using Market.Application.Modules.Auth.Commands.Login;
using Market.Application.Modules.Auth.Commands.Logout;
using Market.Application.Modules.Auth.Commands.Refresh;
using Microsoft.AspNetCore.Antiforgery;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(IMediator mediator, IAntiforgery antiforgery, IHostEnvironment env) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken ct)
    {
        var tokens = await mediator.Send(command, ct);

        Response.SetAccessToken(env, tokens.AccessToken, tokens.AccessTokenExpiresAtUtc);
        Response.SetRefreshToken(env, tokens.RefreshToken, tokens.RefreshTokenExpiresAtUtc);
        // The XSRF-TOKEN cookie is minted on GET /User/me instead of here: this request is still
        // anonymous (the access_token cookie we just set doesn't retroactively authenticate it),
        // and antiforgery embeds the caller's identity in the token, so minting it now would make
        // it fail validation later against an authenticated request (e.g. Logout).

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
            Console.WriteLine($"[DEBUG ANTIFORGERY] -> {ex.Message}");
            return false;
        }
    }
}
