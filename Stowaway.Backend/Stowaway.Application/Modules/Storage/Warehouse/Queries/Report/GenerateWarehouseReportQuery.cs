namespace Stowaway.Application.Modules.Storage.Warehouse.Queries.Report
{
    public sealed class GenerateWarehouseReportQuery : IRequest<GenerateWarehouseReportResult>
    {
        public int WarehouseId { get; set; }
        public string? ReportType { get; set; }

        // Null/empty means "all columns" (the PDF's previous, only behavior).
        public List<string>? ContainerColumns { get; set; }
        public List<string>? ItemColumns { get; set; }
    }
}
