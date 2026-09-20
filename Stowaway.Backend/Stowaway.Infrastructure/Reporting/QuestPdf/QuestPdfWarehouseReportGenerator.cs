using Stowaway.Application.Abstractions.Reporting;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Stowaway.Infrastructure.Reporting.QuestPdf
{
    public class QuestPdfWarehouseReportGenerator : IPdfReportGenerator
    {
        private sealed record ColumnDef<TRow>(string Key, string Header, float Width, Func<TRow, string> Value);

        private static readonly ColumnDef<WarehouseReportContainerRow>[] ContainerColumnDefs =
        {
            new(WarehouseReportColumns.ContainerLocation, "Location", 2.4f, r => r.AncestorPath),
            new(WarehouseReportColumns.ContainerType, "Type", 2f, r => r.TypeName),
            new(WarehouseReportColumns.ContainerItems, "Items", 1.1f, r => $"{r.ItemsUsed} / {r.MaxItems}"),
            new(WarehouseReportColumns.ContainerContainers, "Containers", 1.2f, r => $"{r.ContainersUsed} / {r.MaxContainers}"),
            new(WarehouseReportColumns.ContainerStatus, "Status", 1.4f, r => r.Status),
        };

        private static readonly ColumnDef<WarehouseReportItemRow>[] ItemColumnDefs =
        {
            new(WarehouseReportColumns.ItemContainer, "Container", 2.4f, r => r.ContainerPath),
            new(WarehouseReportColumns.ItemQuantity, "Quantity", 1f, r => r.Quantity.ToString()),
            new(WarehouseReportColumns.ItemSupplier, "Supplier", 1.6f, r => r.SupplierName),
            new(WarehouseReportColumns.ItemTags, "Tags", 2f, r => r.Tags),
        };

        public QuestPdfWarehouseReportGenerator()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public byte[] GenerateWarehouseReport(WarehouseReportData data)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(9));

                    page.Header().Column(header =>
                    {
                        header.Item().Text(data.WarehouseName).FontSize(20).Bold();
                        header.Item().Text(data.ReportTypeLabel).FontSize(12).FontColor(Colors.Grey.Darken2);
                        header.Item().PaddingTop(2)
                            .Text($"Generated {data.GeneratedAtUtc:yyyy-MM-dd HH:mm} UTC")
                            .FontSize(9).FontColor(Colors.Grey.Medium);
                    });

                    page.Content().PaddingTop(15).Column(content =>
                    {
                        content.Spacing(18);

                        if (data.ShowContainers)
                        {
                            var columns = ContainerColumnDefs.Where(c => data.ContainerColumns.Contains(c.Key)).ToList();
                            content.Item().Column(section => ComposeSection(
                                section, "Containers", data.Containers, columns, r => r.Name));
                        }

                        if (data.ShowItems)
                        {
                            var columns = ItemColumnDefs.Where(c => data.ItemColumns.Contains(c.Key)).ToList();
                            content.Item().Column(section => ComposeSection(
                                section, "Items", data.Items, columns, r => r.Name));
                        }
                    });

                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.CurrentPageNumber();
                        text.Span(" / ");
                        text.TotalPages();
                    });
                });
            });

            return document.GeneratePdf();
        }

        private static void ComposeSection<TRow>(
            QuestPDF.Fluent.ColumnDescriptor section,
            string title,
            IReadOnlyList<TRow> rows,
            IReadOnlyList<ColumnDef<TRow>> columns,
            Func<TRow, string> nameSelector)
        {
            section.Item().Text($"{title} ({rows.Count})").FontSize(13).Bold();

            if (rows.Count == 0)
            {
                section.Item().Text($"No {title.ToLowerInvariant()} in this warehouse.").FontColor(Colors.Grey.Darken1);
                return;
            }

            section.Item().Table(table =>
            {
                table.ColumnsDefinition(cols =>
                {
                    cols.RelativeColumn(2.2f); // Name - always shown, first
                    foreach (var column in columns)
                    {
                        cols.RelativeColumn(column.Width);
                    }
                });

                table.Header(header =>
                {
                    header.Cell().Element(HeaderCell).Text("Name");
                    foreach (var column in columns)
                    {
                        header.Cell().Element(HeaderCell).Text(column.Header);
                    }
                });

                foreach (var row in rows)
                {
                    table.Cell().Element(BodyCell).Text(nameSelector(row));
                    foreach (var column in columns)
                    {
                        table.Cell().Element(BodyCell).Text(column.Value(row));
                    }
                }
            });
        }

        private static IContainer HeaderCell(IContainer container) => container
            .DefaultTextStyle(x => x.Bold())
            .PaddingVertical(4)
            .BorderBottom(1)
            .BorderColor(Colors.Grey.Darken2);

        private static IContainer BodyCell(IContainer container) => container
            .PaddingVertical(3)
            .BorderBottom(0.5f)
            .BorderColor(Colors.Grey.Lighten2);
    }
}
