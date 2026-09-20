using Stowaway.Application.Abstractions.Reporting;
using Stowaway.Application.Common.Exceptions;
using Stowaway.Application.Modules.Storage.Warehouse.Queries.Report;
using Stowaway.Domain.Entities.Storage;
using Stowaway.Tests.Helpers;

namespace Stowaway.Tests.UnitTests;

public class GenerateWarehouseReportQueryHandlerTests
{
    private sealed class FakePdfReportGenerator : IPdfReportGenerator
    {
        public WarehouseReportData? CapturedData { get; private set; }

        public byte[] GenerateWarehouseReport(WarehouseReportData data)
        {
            CapturedData = data;
            return new byte[] { 1, 2, 3 };
        }
    }

    private static (WarehouseEntity warehouse, ContainerEntity parent, ContainerEntity child, SupplierEntity supplier) SeedStorage(TestDbContext db)
    {
        var warehouse = new WarehouseEntity { Id = 1, Name = "Main Warehouse", Description = "d", City = "c", Address = "a", Capacity = 100, isEnabled = true };
        var containerType = new ContainerTypeEntity { Id = 1, MaxItems = 10, MaxContainers = 5, Price = 10m };
        var parent = new ContainerEntity { Id = 1, Name = "Shelf A", WarehouseId = 1, ContainerTypeId = containerType.Id };
        var child = new ContainerEntity { Id = 2, Name = "Bin A1", WarehouseId = 1, ContainerTypeId = containerType.Id, ParentContainerId = 1 };
        var supplier = new SupplierEntity { Id = 1, Name = "Acme", Description = "d", Address = "a" };

        db.Warehouses.Add(warehouse);
        db.ContainerTypes.Add(containerType);
        db.Containers.AddRange(parent, child);
        db.Suppliers.Add(supplier);

        return (warehouse, parent, child, supplier);
    }

    [Fact]
    public async Task Handle_BuildsReportData_WithContainerHierarchyAndItems()
    {
        await using var db = TestDbContext.Create();
        var (_, parent, child, supplier) = SeedStorage(db);
        db.Item.Add(new ItemEntity { Name = "Widget", Description = "d", Quantity = 3, ContainerId = child.Id, SupplierId = supplier.Id });
        await db.SaveChangesAsync(CancellationToken.None);

        var pdfGenerator = new FakePdfReportGenerator();
        var handler = new GenerateWarehouseReportQueryHandler(db, new FakeCurrentUser { IsAdmin = true }, pdfGenerator);

        var result = await handler.Handle(new GenerateWarehouseReportQuery { WarehouseId = 1, ReportType = "both" }, CancellationToken.None);

        Assert.NotNull(pdfGenerator.CapturedData);
        var data = pdfGenerator.CapturedData!;

        Assert.Equal("Main Warehouse", data.WarehouseName);
        Assert.True(data.ShowContainers);
        Assert.True(data.ShowItems);

        Assert.Equal(2, data.Containers.Count);
        var parentRow = Assert.Single(data.Containers, r => r.Name == "Shelf A");
        Assert.Equal("—", parentRow.AncestorPath);
        var childRow = Assert.Single(data.Containers, r => r.Name == "Bin A1");
        Assert.Equal("Shelf A", childRow.AncestorPath);
        Assert.Equal(3, childRow.ItemsUsed);

        var itemRow = Assert.Single(data.Items);
        Assert.Equal("Widget", itemRow.Name);
        Assert.Equal("Shelf A / Bin A1", itemRow.ContainerPath);
        Assert.Equal("Acme", itemRow.SupplierName);

        Assert.NotEmpty(result.FileContent);
        Assert.EndsWith(".pdf", result.FileName);
    }

    [Fact]
    public async Task Handle_ContainersOnly_OmitsItems()
    {
        await using var db = TestDbContext.Create();
        var (_, _, child, supplier) = SeedStorage(db);
        db.Item.Add(new ItemEntity { Name = "Widget", Description = "d", Quantity = 1, ContainerId = child.Id, SupplierId = supplier.Id });
        await db.SaveChangesAsync(CancellationToken.None);

        var pdfGenerator = new FakePdfReportGenerator();
        var handler = new GenerateWarehouseReportQueryHandler(db, new FakeCurrentUser { IsAdmin = true }, pdfGenerator);

        await handler.Handle(new GenerateWarehouseReportQuery { WarehouseId = 1, ReportType = "containers" }, CancellationToken.None);

        var data = pdfGenerator.CapturedData!;
        Assert.True(data.ShowContainers);
        Assert.False(data.ShowItems);
        Assert.Empty(data.Items);
        Assert.Equal(2, data.Containers.Count);
    }

    [Fact]
    public async Task Handle_DefaultsToAllColumns_WhenNoneRequested()
    {
        await using var db = TestDbContext.Create();
        SeedStorage(db);
        await db.SaveChangesAsync(CancellationToken.None);

        var pdfGenerator = new FakePdfReportGenerator();
        var handler = new GenerateWarehouseReportQueryHandler(db, new FakeCurrentUser { IsAdmin = true }, pdfGenerator);

        await handler.Handle(new GenerateWarehouseReportQuery { WarehouseId = 1 }, CancellationToken.None);

        var data = pdfGenerator.CapturedData!;
        Assert.Equal(WarehouseReportColumns.AllContainerColumns, data.ContainerColumns);
        Assert.Equal(WarehouseReportColumns.AllItemColumns, data.ItemColumns);
    }

    [Fact]
    public async Task Handle_RestrictsColumns_WhenRequested()
    {
        await using var db = TestDbContext.Create();
        SeedStorage(db);
        await db.SaveChangesAsync(CancellationToken.None);

        var pdfGenerator = new FakePdfReportGenerator();
        var handler = new GenerateWarehouseReportQueryHandler(db, new FakeCurrentUser { IsAdmin = true }, pdfGenerator);

        await handler.Handle(new GenerateWarehouseReportQuery
        {
            WarehouseId = 1,
            ContainerColumns = new List<string> { WarehouseReportColumns.ContainerStatus },
            ItemColumns = new List<string> { WarehouseReportColumns.ItemQuantity },
        }, CancellationToken.None);

        var data = pdfGenerator.CapturedData!;
        Assert.Equal(new[] { WarehouseReportColumns.ContainerStatus }, data.ContainerColumns);
        Assert.Equal(new[] { WarehouseReportColumns.ItemQuantity }, data.ItemColumns);
    }

    [Fact]
    public async Task Handle_ThrowsNotFound_WhenWarehouseDoesNotExist()
    {
        await using var db = TestDbContext.Create();
        var handler = new GenerateWarehouseReportQueryHandler(db, new FakeCurrentUser { IsAdmin = true }, new FakePdfReportGenerator());

        await Assert.ThrowsAsync<StowawayNotFoundException>(() =>
            handler.Handle(new GenerateWarehouseReportQuery { WarehouseId = 999 }, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ThrowsNotFound_WhenNonAdminUserHasNoAccessToWarehouse()
    {
        await using var db = TestDbContext.Create();
        SeedStorage(db);
        await db.SaveChangesAsync(CancellationToken.None);

        var handler = new GenerateWarehouseReportQueryHandler(db, new FakeCurrentUser { IsAdmin = false, UserId = 42 }, new FakePdfReportGenerator());

        await Assert.ThrowsAsync<StowawayNotFoundException>(() =>
            handler.Handle(new GenerateWarehouseReportQuery { WarehouseId = 1 }, CancellationToken.None));
    }
}
