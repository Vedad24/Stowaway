namespace Market.Application.Common;

public sealed class PageResult<T>
{
    public int Total { get; init; }
    public IReadOnlyList<T> Items { get; init; }
    public int TotalPages { get; set; }

    /// <summary>
    /// Creates a PageResult from an IQueryable using EF Core asynchronous methods.
    /// </summary>
    public static async Task<PageResult<T>> FromQueryableAsync(
        IQueryable<T> query,
        PageRequest paging,
        CancellationToken ct = default,
        bool includeTotal = true)
    {
        int total = 0;
        if (includeTotal)
            total = await query.CountAsync(ct);

        var items = await query
            .Skip(paging.SkipCount)
            .Take(paging.PageSize)
            .ToListAsync(ct);
        var totalPages = (int)Math.Ceiling((double)total / paging.PageSize);
        return new PageResult<T> { Total = total, Items = items, TotalPages = totalPages };
    }
}