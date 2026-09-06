using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Stowaway.Tests;

public class UsersControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;

    public UsersControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateUser_InvalidEmail_ReturnsBadRequest()
    {
        var client = _factory.CreateClient();

        var request = new
        {
            Email = "not-an-email",
            Password = "Password123!",
            FirstName = "Test",
            LastName = "User"
        };

        var response = await client.PostAsJsonAsync("/User", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
