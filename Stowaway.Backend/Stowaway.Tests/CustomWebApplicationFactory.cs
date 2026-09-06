using System.Net.Http.Json;

namespace Stowaway.Tests;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("IntegrationTests");
    }

    public async Task<HttpClient> GetAuthenticatedClientAsync()
    {
        // Login sets the access/refresh tokens as httpOnly cookies on the response;
        // WebApplicationFactory's client tracks cookies automatically (HandleCookies
        // defaults to true), so no manual Authorization header is needed.
        var client = CreateClient();

        var loginRequest = new
        {
            Email = "test",
            Password = "test123"
        };

        var response = await client.PostAsJsonAsync("api/auth/login", loginRequest);
        response.EnsureSuccessStatusCode();

        return client;
    }
}