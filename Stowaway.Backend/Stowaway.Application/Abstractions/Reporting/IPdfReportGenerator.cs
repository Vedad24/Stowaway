namespace Stowaway.Application.Abstractions.Reporting
{
    public interface IPdfReportGenerator
    {
        byte[] GenerateWarehouseReport(WarehouseReportData data);
    }
}
