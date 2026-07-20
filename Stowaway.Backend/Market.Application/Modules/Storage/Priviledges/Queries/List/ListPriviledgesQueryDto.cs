namespace Market.Application.Modules.Storage.Priviledges.Queries.List
{
    public sealed class ListPriviledgesQueryDto
    {
        public int Id { get; init; }

        public string Name { get; init; } = string.Empty;

        public string? Description { get; init; }
    }
}