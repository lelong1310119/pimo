namespace PismoWebInput.Core.Infrastructure.Common.Paging;

public interface IInfiniteList<T>
{
    IEnumerable<T> Items { get; init; }
    int? Take { get; set; }
    int? Next { get; init; }
}

public class InfiniteList<T> : IInfiniteList<T>
{
    public IEnumerable<T> Items { get; init; }
    public int? Take { get; set; }
    public int? Next { get; init; }
}