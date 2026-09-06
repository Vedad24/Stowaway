using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace Stowaway.Tests;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("IntegrationTests");

        // Stripe:ApiKey/WebhookSecret have no value in appsettings.json - they're only ever
        // supplied via user-secrets/env vars on a dev machine - but StripeOptions.ValidateOnStart()
        // still requires them, so the test host can't boot at all without these. Tests never
        // exercise real Stripe calls, so dummy values are enough to satisfy validation.
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Stripe:ApiKey"] = "sk_test_dummy",
                ["Stripe:WebhookSecret"] = "whsec_dummy"
            });
        });
    }

    public async Task<HttpClient> GetAuthenticatedClientAsync()
        => await GetAuthenticatedClientAsync("test", "test123");

    public async Task<HttpClient> GetAuthenticatedClientAsync(string email, string password)
    {
        // Login sets the access/refresh tokens as httpOnly cookies on the response;
        // WebApplicationFactory's client tracks cookies automatically (HandleCookies
        // defaults to true), so no manual Authorization header is needed.
        // AuthCookies marks them Secure outside Development, and CreateClient()'s default
        // http:// base address makes CookieContainer silently drop Secure cookies before the
        // next request - the in-memory TestServer never does real TLS, so an https:// base
        // address is enough to satisfy that flag without changing any server-side behavior.
        var client = CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });

        var loginRequest = new
        {
            Email = email,
            Password = password
        };

        var response = await client.PostAsJsonAsync("api/auth/login", loginRequest);
        response.EnsureSuccessStatusCode();

        return client;
    }
}