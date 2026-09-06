namespace Stowaway.Application.Modules.Storage.StorageIdentity.Queries.List
{
    public sealed class ListPriviledgeGroupsQueryValidator : AbstractValidator<ListPriviledgeGroupsQuery>
    {
        public ListPriviledgeGroupsQueryValidator()
        {
            RuleFor(x => x.WarehouseId)
                .GreaterThan(0)
                .When(x => x.WarehouseId.HasValue);
        }
    }
}
