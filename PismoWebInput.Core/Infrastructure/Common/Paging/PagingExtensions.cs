using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;

namespace PismoWebInput.Core.Infrastructure.Common.Paging;

public static class PagingExtensions
{
    public const int MaxPageSize = 100;
    public const int DefaultPageSize = 20;

    public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> query,
        IOrderingParams param,
        string defaultSort = "Id",
        params string[] allowedSortColumns)
    {
        var ordering = !allowedSortColumns.Any() ||
                       allowedSortColumns.Contains(param.Sort?.Trim(), StringComparer.OrdinalIgnoreCase)
            ? param.GetOrderBy(defaultSort)
            : defaultSort;
        return query.OrderBy(ordering);
    }

    public static string GetOrderBy(this IOrderingParams ordering, string defaultSort = "Id")
    {
        var sort = ordering.Sort;
        if (string.IsNullOrWhiteSpace(sort)) sort = defaultSort;

        return ordering.IsDescending() ? $"{sort} DESC" : sort;
    }

    public static bool IsDescending(this IOrderingParams ordering)
    {
        return string.Equals(ordering.Order, "desc", StringComparison.OrdinalIgnoreCase);
    }

    public static int GetPageSize(this IPagingParams paging)
    {
        var pageSize = paging.PageSize.GetValueOrDefault(DefaultPageSize);
        if (pageSize > MaxPageSize) pageSize = MaxPageSize;

        return pageSize;
    }

    public static int GetPage(this IPagingParams paging)
    {
        return paging.Page.GetValueOrDefault(1);
    }

    public static IQueryable<T> Paging<T>(this IQueryable<T> query, IPagingParams param)
    {
        var page = param.GetPage();
        var pageSize = param.GetPageSize();
        return query.Skip((page - 1) * pageSize).Take(pageSize);
    }


    #region PagedList Extensions

    public static async Task<IPagedList<T>> PageByAsync<T>(this IQueryable<T> query, int page, int pageSize)
    {
        var totalCount = await query.CountAsync();
        var list = await query.ToPagedListAsync(page, pageSize, totalCount);
        return list;
    }

    public static Task<IPagedList<T>> PageByAsync<T>(this IQueryable<T> query, IPagingParams param)
    {
        return query.PageByAsync(param.GetPage(), param.GetPageSize());
    }

    public static Task<IPagedList<TResult>> PageByAsync<T, TResult>(this IQueryable<T> query, IPagingParams param,
        Func<IEnumerable<T>, IList<TResult>> mapping)
    {
        return query.PageByAsync(param.GetPage(), param.GetPageSize(), mapping);
    }

    public static async Task<IPagedList<TResult>> PageByAsync<T, TResult>(this IQueryable<T> query, int page,
        int pageSize, Func<IEnumerable<T>, IList<TResult>> mapping)
    {
        var list = await query.PageByAsync(page, pageSize);
        return mapping(list).ToPagedList(list.GetMetaData());
    }

    public static async Task<IPagedList<T>> ToPagedListAsync<T>(this IQueryable<T> superset, int pageNumber,
        int pageSize, int totalCount)
    {
        var list = new List<T>();
        if (superset != null && totalCount > 0)
            list.AddRange(pageNumber == 1
                ? await superset.Take(pageSize).ToListAsync()
                : await superset.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync());

        return new Pagination<T>(list, pageNumber, pageSize, totalCount);
    }

    public static IPagedList<T> ToPagedList<T>(this IQueryable<T> superset, int pageNumber, int pageSize,
        int totalCount)
    {
        var list = new List<T>();
        if (superset != null && totalCount > 0)
            list.AddRange(pageNumber == 1
                ? superset.Take(pageSize).ToList()
                : superset.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList());

        return new Pagination<T>(list, pageNumber, pageSize, totalCount);
    }

    public static IPagedList<T> ToPagedList<T>(this IList<T> superset, IPagedList metadata)
    {
        return new Pagination<T>(superset, metadata);
    }

    public static IPaginationList<T> ToPaginationList<T>(this IPagedList<T> source)
    {
        return new PaginationList<T>(source);
    }

    public static PaginationMeta GetMetaData(this IPagedList pagedList)
    {
        return new PaginationMeta(pagedList);
    }

    #endregion

    #region Infinite paging

    public static int GetSkip(this IInfiniteParams param)
    {
        return param.Skip.GetValueOrDefault(0);
    }

    public static int GetTake(this IInfiniteParams param)
    {
        var limit = param.Take.GetValueOrDefault(DefaultPageSize);
        if (limit > MaxPageSize) limit = MaxPageSize;

        return limit;
    }

    public static async Task<IInfiniteList<T>> ToInfiniteListAsync<T>(this IQueryable<T> query, int skip, int take)
    {
        var list = new List<T>();
        if (query != null) list.AddRange(await query.Skip(skip).Take(take).ToListAsync());

        return new InfiniteList<T> { Items = list, Next = list.Any() ? skip + take : null, Take = take };
    }

    public static Task<IInfiniteList<TResult>> ToInfiniteListAsync<T, TResult>(this IQueryable<T> query,
        IInfiniteParams param, Func<IEnumerable<T>, IList<TResult>> mapping)
    {
        return query.ToInfiniteListAsync(param.GetSkip(), param.GetTake(), mapping);
    }

    public static Task<IInfiniteList<T>> ToInfiniteListAsync<T>(this IQueryable<T> query, IInfiniteParams param)
    {
        return query.ToInfiniteListAsync(param.GetSkip(), param.GetTake());
    }

    public static async Task<IInfiniteList<TResult>> ToInfiniteListAsync<T, TResult>(this IQueryable<T> query,
        int skip,
        int take,
        Func<IEnumerable<T>, IList<TResult>> mapping)
    {
        var list = await query.ToInfiniteListAsync(skip, take);
        return new InfiniteList<TResult>
        {
            Items = mapping(list.Items),
            Next = list.Next,
            Take = list.Take
        };
    }

    #endregion
}