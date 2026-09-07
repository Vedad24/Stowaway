namespace Stowaway.Application.Modules.Dashboard.Queries.GetGlobalStats
{
    public sealed class GetGlobalStatsQueryDto
    {
        public int TotalUsers { get; init; }
        public int TotalWarehouses { get; init; }
        public int TotalContainers { get; init; }
        public int TotalItems { get; init; }
        public int TotalSuppliers { get; init; }
        public int TotalOrders { get; init; }
    }
}
