namespace Stowaway.Application.Modules.Storage.Warehouse.Queries.Report
{
    public sealed class GenerateWarehouseReportResult
    {
        public required byte[] FileContent { get; set; }
        public required string FileName { get; set; }
    }
}
