using Stowaway.Application.Abstractions.Reporting;

namespace Stowaway.Application.Modules.Storage.Warehouse.Queries.Report
{
    public sealed class GenerateWarehouseReportQueryValidator : AbstractValidator<GenerateWarehouseReportQuery>
    {
        private static readonly string[] AllowedTypes = { "containers", "items", "both" };

        public GenerateWarehouseReportQueryValidator()
        {
            RuleFor(x => x.WarehouseId)
                .GreaterThan(0).WithMessage("WarehouseId is required.");

            RuleFor(x => x.ReportType)
                .Must(type => string.IsNullOrWhiteSpace(type) || AllowedTypes.Contains(type.ToLowerInvariant()))
                .WithMessage("Report type must be 'containers', 'items', or 'both'.");

            RuleForEach(x => x.ContainerColumns)
                .Must(c => WarehouseReportColumns.AllContainerColumns.Contains(c))
                .WithMessage($"Container columns must be one of: {string.Join(", ", WarehouseReportColumns.AllContainerColumns)}.");

            RuleForEach(x => x.ItemColumns)
                .Must(c => WarehouseReportColumns.AllItemColumns.Contains(c))
                .WithMessage($"Item columns must be one of: {string.Join(", ", WarehouseReportColumns.AllItemColumns)}.");
        }
    }
}
