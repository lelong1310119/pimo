using System.Collections;

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
/// <seealso cref="IPagedList" />
/// <seealso cref="List{T}" />
public abstract class BasePagination<T> : PaginationMeta, IPagedList<T>
{
    protected readonly List<T> Subset = new();

    protected BasePagination()
    {
    }

    protected BasePagination(IPagedList pagedList)
        : base(pagedList)
    {
    }

    #region IPagedList<T> Members

    /// <summary>
    ///     Returns an enumerator that iterates through the BasePagedList&lt;T&gt;.
    /// </summary>
    /// <returns>A BasePagedList&lt;T&gt;.Enumerator for the BasePagedList&lt;T&gt;.</returns>
    public IEnumerator<T> GetEnumerator()
    {
        return Subset.GetEnumerator();
    }

    /// <summary>
    ///     Returns an enumerator that iterates through the BasePagedList&lt;T&gt;.
    /// </summary>
    /// <returns>A BasePagedList&lt;T&gt;.Enumerator for the BasePagedList&lt;T&gt;.</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    ///     Gets the element at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    public T this[int index] => Subset[index];

    /// <summary>
    ///     Gets the number of elements contained on this page.
    /// </summary>
    public virtual int Count => Subset.Count;

    #endregion
}