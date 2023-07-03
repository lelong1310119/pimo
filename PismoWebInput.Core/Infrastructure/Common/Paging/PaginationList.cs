namespace PismoWebInput.Core.Infrastructure.Common.Paging;

public interface IPaginationList<out T>
{
    IEnumerable<T> Items { get; }
    PaginationMeta Meta { get; }
}

public class PaginationList<T> : IPaginationList<T>
{
    public PaginationList(IPagedList<T> pagedList)
    {
        Items = pagedList;
        Meta = pagedList.GetMetaData();
    }

    public IEnumerable<T> Items { get; }
    public PaginationMeta Meta { get; }
}