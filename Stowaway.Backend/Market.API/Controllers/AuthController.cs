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
        antiforgery.GetAndStoreTokens(HttpContext);

        return Ok();
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh(CancellationToken ct)
    {
        if (!Request.Cookies.TryGetValue(AuthCookies.RefreshTokenCookieName, out var refreshToken) || string.IsNullOrEmpty(refreshToken))
            return Unauthorized();

        if (!await TryValidateAntiforgeryAsync())
            return Forbid();

        var tokens = await mediator.Send(new RefreshTokenCommand { RefreshToken = refreshToken }, ct);

        Response.SetAccessToken(env, tokens.AccessToken, tokens.AccessTokenExpiresAtUtc);
        Response.SetRefreshToken(env, tokens.RefreshToken, tokens.RefreshTokenExpiresAtUtc);
        antiforgery.GetAndStoreTokens(HttpContext);

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
        catch (AntiforgeryValidationException)
        {
            return false;
        }
    }
}
