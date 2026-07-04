using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Extensions;

public static class QueryableExtensions
{
    public static async Task<PagedList<T>> ToPagedListAsync<T>(
        this IQueryable<T> source,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        int count = await source.CountAsync(cancellationToken);
        List<T> items = await source.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        return new PagedList<T>(items, count, page, pageSize);
    }
}
