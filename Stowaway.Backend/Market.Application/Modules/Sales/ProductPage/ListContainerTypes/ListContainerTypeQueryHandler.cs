namespace Stowaway.Application.Modules.Sales.ProductPage.ListContainerTypes
{
    public sealed class ListContainerTypeQueryHandler(IAppDbContext ctx)
        : IRequestHandler<ListContainerTypeQuery, PageResult<ListContainerTypeQueryDto>>
    {
        public async Task<PageResult<ListContainerTypeQueryDto>> Handle(ListContainerTypeQuery request, CancellationToken cancellationToken)
        {
            var query = ctx.ContainerTypes.AsNoTracking();

            

            var projectedQuery = query.Select(x => new ListContainerTypeQueryDto
            {
                Id = x.Id,
                DisplayName = x.ToString(),
                MaxItems = x.MaxItems,
                MaxContainers = x.MaxContainers,
                Price = x.Price
            });

            return await PageResult<ListContainerTypeQueryDto>.FromQueryableAsync(projectedQuery, request.Paging, cancellationToken);
        }
    }
}
