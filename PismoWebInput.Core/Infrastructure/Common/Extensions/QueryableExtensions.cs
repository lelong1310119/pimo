using System.Linq.Expressions;

namespace PismoWebInput.Core.Infrastructure.Common.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition,
        Expression<Func<T, bool>> predicate)
    {
        return condition ? query.Where(predicate) : query;
    }

    public static IQueryable<T> If<T>(this IQueryable<T> source, bool condition,
        Func<IQueryable<T>, IQueryable<T>> transform)
    {
        return condition ? transform(source) : source;
    }
}