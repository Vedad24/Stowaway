namespace Stowaway.Application.Abstractions.Reporting
{
    // Name is always included in both tables and isn't a selectable column - single source of
    // truth for the frontend's checkbox options, the query validator, and the PDF generator's
    // default ("nothing selected" -> show everything) behavior.
    public static class WarehouseReportColumns
    {
        public const string ContainerLocation = "location";
        public const string ContainerType = "type";
        public const string ContainerItems = "items";
        public const string ContainerContainers = "containers";
        public const string ContainerStatus = "status";

        public static readonly string[] AllContainerColumns =
        {
            ContainerLocation,
            ContainerType,
            ContainerItems,
            ContainerContainers,
            ContainerStatus,
        };

        public const string ItemContainer = "container";
        public const string ItemQuantity = "quantity";
        public const string ItemSupplier = "supplier";
        public const string ItemTags = "tags";

        public static readonly string[] AllItemColumns =
        {
            ItemContainer,
            ItemQuantity,
            ItemSupplier,
            ItemTags,
        };
    }
}
