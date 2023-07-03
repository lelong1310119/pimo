namespace PismoWebInput.Core.Infrastructure.Common.Paging;

/// <summary>
///     Represents a subset of a collection of objects that can be individually accessed by index and containing metadata
///     about the superset collection of objects this subset was created from.
/// </summary>
/// <remarks>
///     Represents a subset of a collection of objects that can be individually accessed by index and containing metadata
///     about the superset collection of objects this subset was created from.
/// </remarks>
/// <typeparam name="T">The type of object the collection should contain.</typeparam>
/// <seealso cref="BasePagination{T}" />
/// <seealso cref="List{T}" />
/// <seealso cref="IPagedList" />
[Serializable]
public class Pagination<T> : BasePagination<T>
{
    public Pagination(IList<T> superset, int pageNumber, int pageSize, int? totalCount = null)
    {
        if (pageNumber < 1)
            throw new ArgumentOutOfRangeException(nameof(pageNumber), pageNumber, "PageNumber cannot be below 1.");
        if (pageSize < 1)
            throw new ArgumentOutOfRangeException(nameof(pageSize), pageSize, "PageSize cannot be less than 1.");

        // set source to blank list if superset is null to prevent exceptions
        TotalItemCount = totalCount ?? (superset?.Count() ?? 0);

        PageSize = pageSize;
        PageNumber = pageNumber;
        PageCount = TotalItemCount > 0
            ? (int)Math.Ceiling(TotalItemCount / (double)PageSize)
            : 0;
        HasPreviousPage = PageNumber > 1;
        HasNextPage = PageNumber < PageCount;
        IsFirstPage = PageNumber == 1;
        IsLastPage = PageNumber >= PageCount;
        FirstItemOnPage = (PageNumber - 1) * PageSize + 1;
        var numberOfLastItemOnPage = FirstItemOnPage + PageSize - 1;
        LastItemOnPage = numberOfLastItemOnPage > TotalItemCount
            ? TotalItemCount
            : numberOfLastItemOnPage;

        if (superset != null) Subset.AddRange(superset);
    }

    public Pagination(IList<T> superset, IPagedList metadata)
        : base(metadata)
    {
        Subset.AddRange(superset);
    }
}