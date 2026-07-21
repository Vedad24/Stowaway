namespace Stowaway.Application.Modules.Sales.ProductPage.ListContainerTypes
{
    public sealed class ListContainerTypeQueryDto
    {
        public required int Id { get; init; }
        public required string DisplayName { get; set; }
        public required int MaxItems { get; init; }
        public required int MaxContainers { get; init; }
        public required decimal Price { get; init; }
    }
}
