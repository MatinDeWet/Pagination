using System.Linq.Expressions;

namespace Pagination;

/// <summary>
/// Provides shared dynamic ordering helpers for IQueryable sources.
/// </summary>
internal static class QueryableOrderingExtensions
{
    /// <summary>
    /// Orders a query in ascending order by a property name.
    /// </summary>
    /// <typeparam name="T">The type of the elements of the source sequence.</typeparam>
    /// <param name="source">The queryable data source.</param>
    /// <param name="propertyName">The name of the property to use for ordering.</param>
    /// <returns>An ordered query sorted in ascending order by the specified property.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the specified property does not exist on type T.
    /// </exception>
    internal static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string propertyName)
    {
        return source.OrderBy(ToLambda<T>(propertyName));
    }

    /// <summary>
    /// Orders a query in descending order by a property name.
    /// </summary>
    /// <typeparam name="T">The type of the elements of the source sequence.</typeparam>
    /// <param name="source">The queryable data source.</param>
    /// <param name="propertyName">The name of the property to use for ordering in descending order.</param>
    /// <returns>An ordered query sorted in descending order by the specified property.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the specified property does not exist on type T.
    /// </exception>
    internal static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string propertyName)
    {
        return source.OrderByDescending(ToLambda<T>(propertyName));
    }

    /// <summary>
    /// Creates a lambda expression that accesses a property by name.
    /// </summary>
    /// <typeparam name="T">The type of the object that contains the property.</typeparam>
    /// <param name="propertyName">The name of the property to access.</param>
    /// <returns>A lambda expression that accesses the specified property and returns it as <see cref="object"/>.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the specified property does not exist on type T.
    /// </exception>
    private static Expression<Func<T, object>> ToLambda<T>(string propertyName)
    {
        try
        {
            ParameterExpression parameter = Expression.Parameter(typeof(T));
            MemberExpression property = Expression.Property(parameter, propertyName);

            Expression propAsObject = property.Type.IsValueType
                ? Expression.Convert(property, typeof(object))
                : property;

            return Expression.Lambda<Func<T, object>>(propAsObject, parameter);
        }
        catch (ArgumentException ex)
        {
            throw new ArgumentException($"Property '{propertyName}' does not exist on type '{typeof(T).Name}'.", nameof(propertyName), ex);
        }
    }
}
