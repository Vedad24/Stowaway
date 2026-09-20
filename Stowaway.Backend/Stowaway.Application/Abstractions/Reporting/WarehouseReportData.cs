namespace Stowaway.Application.Abstractions.Reporting
{
    public sealed record WarehouseReportData(
        string WarehouseName,
        string ReportTypeLabel,
        DateTime GeneratedAtUtc,
        bool ShowContainers,
        bool ShowItems,
        IReadOnlyList<WarehouseReportContainerRow> Containers,
        IReadOnlyList<WarehouseReportItemRow> Items,
        IReadOnlyList<string> ContainerColumns,
        IReadOnlyList<string> ItemColumns);

    public sealed record WarehouseReportContainerRow(
        string Name,
        string AncestorPath,
        string TypeName,
        int ItemsUsed,
        int MaxItems,
        int ContainersUsed,
        int MaxContainers,
        string Status);

    public sealed record WarehouseReportItemRow(
        string Name,
        string ContainerPath,
        int Quantity,
        string SupplierName,
        string Tags);
}
