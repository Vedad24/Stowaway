using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Stowaway.Application.Modules.Storage.Supplier.Commands.Create;
using Stowaway.Application.Modules.Storage.Supplier.Commands.Update;
using Stowaway.Application.Modules.Storage.Supplier.Queries.GetById;
using Stowaway.Application.Modules.Storage.Supplier.Queries.List;
using Xunit;

namespace Stowaway.Tests;

// End-to-end HTTP flows through the real MVC pipeline (auth, [HasPermission], MediatR,
// EF InMemory), as opposed to the handler-level UnitTests which bypass all of that.
public class SupplierFlowTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly CustomWebApplicationFactory<Program> _factory;

    public SupplierFlowTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task SupplierFlow_CreateGetUpdateDelete_FullLifecycle()
    {
        var client = await _factory.GetAuthenticatedClientAsync("manager@market.local", "Manager123!");

        var uniqueName = $"Test Supplier {Guid.NewGuid()}";
        var create = await client.PostAsJsonAsync("Supplier", new CreateSupplierCommand
        {
            Name = uniqueName,
            Description = "Created by integration test",
            Address = "1 Test Street",
            TotalDeliveries = 10,
            FailedDeliveries = 2
        });
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);

        var created = await create.Content.ReadFromJsonAsync<CreatedSupplierDto>(JsonOptions);
        Assert.NotNull(created);
        Assert.True(created!.Id > 0);
        var id = created.Id;

        var afterCreate = await client.GetFromJsonAsync<GetSupplierByIdQueryDto>($"Supplier/{id}", JsonOptions);
        Assert.NotNull(afterCreate);
        Assert.Equal(uniqueName, afterCreate!.Name);
        Assert.Equal("Created by integration test", afterCreate.Description);
        Assert.Equal("1 Test Street", afterCreate.Address);
        Assert.Equal(10, afterCreate.TotalDeliveries);
        Assert.Equal(2, afterCreate.FailedDeliveries);

        var updatedName = $"{uniqueName} Updated";
        var update = await client.PutAsJsonAsync($"Supplier/{id}", new UpdateSupplierCommand
        {
            Name = updatedName,
            Description = "Updated by integration test",
            Address = "2 Test Avenue",
            TotalDeliveries = 20,
            FailedDeliveries = 5
        });
        Assert.True(update.IsSuccessStatusCode, $"Update failed with {update.StatusCode}");

        var afterUpdate = await client.GetFromJsonAsync<GetSupplierByIdQueryDto>($"Supplier/{id}", JsonOptions);
        Assert.NotNull(afterUpdate);
        Assert.Equal(updatedName, afterUpdate!.Name);
        Assert.Equal("Updated by integration test", afterUpdate.Description);
        Assert.Equal("2 Test Avenue", afterUpdate.Address);
        Assert.Equal(20, afterUpdate.TotalDeliveries);
        Assert.Equal(5, afterUpdate.FailedDeliveries);

        // No items reference this freshly-created supplier, so DeleteSupplierCommandHandler
        // should let the delete through (unlike the seeded suppliers, which all have items).
        var delete = await client.DeleteAsync($"Supplier/{id}");
        Assert.Equal(HttpStatusCode.OK, delete.StatusCode);

        var afterDelete = await client.GetAsync($"Supplier/{id}");
        Assert.Equal(HttpStatusCode.NotFound, afterDelete.StatusCode);
    }

    [Fact]
    public async Task SupplierFlow_BusinessRuleAndPermissionViolations_ReturnExpectedErrors()
    {
        var managerClient = await _factory.GetAuthenticatedClientAsync("manager@market.local", "Manager123!");

        // Duplicate name - "AquaTerm" is seeded by DynamicDataSeeder.SeedSupplierAsync -
        // CreateSupplierCommandHandler rejects it as a conflict.
        var duplicateName = await managerClient.PostAsJsonAsync("Supplier", new CreateSupplierCommand
        {
            Name = "AquaTerm",
            Description = "Should not be allowed",
            Address = "Nowhere",
            TotalDeliveries = 0,
            FailedDeliveries = 0
        });
        Assert.Equal(HttpStatusCode.Conflict, duplicateName.StatusCode);

        // Blank name - CreateSupplierCommandHandler throws StowawayBusinessRuleException,
        // which the global exception handler also maps to 409 (not 400).
        var blankName = await managerClient.PostAsJsonAsync("Supplier", new CreateSupplierCommand
        {
            Name = "   ",
            Description = "Should not be allowed",
            Address = "Nowhere",
            TotalDeliveries = 0,
            FailedDeliveries = 0
        });
        Assert.Equal(HttpStatusCode.Conflict, blankName.StatusCode);

        // Deleting a supplier that still has items sourced from it is blocked with a
        // ValidationException (400), not silently allowed or treated as a conflict.
        var lookup = await managerClient.GetFromJsonAsync<PagedSuppliers>("Supplier?Search=AquaTerm", JsonOptions);
        var aquaTerm = Assert.Single(lookup!.Items);

        var blockedDelete = await managerClient.DeleteAsync($"Supplier/{aquaTerm.Id}");
        Assert.Equal(HttpStatusCode.BadRequest, blockedDelete.StatusCode);

        // The supplier must still exist - the blocked delete must not have removed it.
        var stillExists = await managerClient.GetAsync($"Supplier/{aquaTerm.Id}");
        Assert.Equal(HttpStatusCode.OK, stillExists.StatusCode);

        // Read-only user ("user@market.local" only has Supplier.Read) must be forbidden
        // from creating a supplier via [HasPermission(Permissions.SupplierCreate)].
        var readOnlyClient = await _factory.GetAuthenticatedClientAsync("user@market.local", "User123!");
        var forbiddenCreate = await readOnlyClient.PostAsJsonAsync("Supplier", new CreateSupplierCommand
        {
            Name = $"Forbidden Supplier {Guid.NewGuid()}",
            Description = "Should not be allowed",
            Address = "Nowhere",
            TotalDeliveries = 0,
            FailedDeliveries = 0
        });
        Assert.Equal(HttpStatusCode.Forbidden, forbiddenCreate.StatusCode);
    }

    private sealed class CreatedSupplierDto
    {
        public int Id { get; set; }
    }

    private sealed class PagedSuppliers
    {
        public List<ListSupplierQueryDto> Items { get; set; } = new();
    }
}
