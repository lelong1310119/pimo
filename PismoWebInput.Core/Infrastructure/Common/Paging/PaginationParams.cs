namespace PismoWebInput.Core.Infrastructure.Common.Paging;

public interface IPagingParams
{
    int? Page { get; set; }
    int? PageSize { get; set; }
}

public interface IOrderingParams
{
    string Sort { get; set; }
    string Order { get; set; }
}

public interface IPaginationParams : IPagingParams, IOrderingParams
{
}

public class PaginationParams : IPaginationParams
{
    public int? Page { get; set; }
    public int? PageSize { get; set; }
    public string Sort { get; set; }
    public string Order { get; set; }
}

public interface IInfiniteParams
{
    int? Skip { get; set; }
    int? Take { get; set; }
}

public class InfiniteParams : IInfiniteParams
{
    public int? Skip { get; set; }
    public int? Take { get; set; }
}