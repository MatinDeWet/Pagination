using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Pagination.Enums;
using Pagination.Models.Requests;
using Pagination.Models.Responses;

namespace Pagination;

/// <summary>
/// Provides extension methods for cursor-based pagination responses.
/// </summary>
public static class CursorPageableExtensions
{
    /// <summary>
    /// Converts an already ordered query into a cursor-based pagination response.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the query.</typeparam>
    /// <param name="query">The ordered queryable data source.</param>
    /// <param name="request">The cursor-based paging request.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="CursorPageableResponse{T}"/> containing page data and cursor metadata.</returns>
    public static async Task<CursorPageableResponse<T>> ToCursorPageableResponseAsync<T>(
        this IOrderedQueryable<T> query,
        CursorPageableRequest request,
        CancellationToken cancellationToken)
    {
        ValidateCursorPageableRequest(request);

        IQueryable<T> pageQuery = query.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Cursor))
        {
            object cursorValue = DecodeCursor(request.Cursor);
            pageQuery = ApplyCursorFilter(pageQuery, request.OrderBy ?? string.Empty, cursorValue, request.OrderDirection);
        }

        List<T> results = await pageQuery.Take(request.PageSize + 1)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        bool hasNextPage = results.Count > request.PageSize;
        IEnumerable<T> data = hasNextPage ? results.Take(request.PageSize) : results;

        CursorPageableResponse<T> response = new()
        {
            Data = data,
            PageSize = request.PageSize,
            HasNextPage = hasNextPage,
            HasPreviousPage = !string.IsNullOrWhiteSpace(request.Cursor),
            OrderBy = request.OrderBy ?? string.Empty,
            OrderDirection = request.OrderDirection
        };

        List<T> dataList = data.ToList();
        if (dataList.Count > 0)
        {
            if (hasNextPage)
            {
                T lastItem = dataList.Last();
                response.NextCursor = EncodeCursor(GetPropertyValue(lastItem, request.OrderBy ?? string.Empty));
            }

            if (!string.IsNullOrWhiteSpace(request.Cursor))
            {
                response.PreviousCursor = null;
            }
        }

        return response;
    }

    /// <summary>
    /// Orders a query by <see cref="BasePaginationRequest.OrderBy"/> and converts it into a cursor-based pagination response.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the query.</typeparam>
    /// <param name="query">The queryable data source.</param>
    /// <param name="request">The cursor request. <see cref="BasePaginationRequest.OrderBy"/> must be provided.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="CursorPageableResponse{T}"/> containing page data and cursor metadata.</returns>
    public static Task<CursorPageableResponse<T>> ToCursorPageableResponseAsync<T>(
        this IQueryable<T> query,
        CursorPageableRequest request,
        CancellationToken cancellationToken)
    {
        ValidateCursorPageableRequest(request);

        ArgumentException.ThrowIfNullOrWhiteSpace(request.OrderBy, nameof(request.OrderBy));

        if (request.OrderDirection == OrderDirectionEnum.Ascending)
        {
            return query.OrderBy(request.OrderBy).ToCursorPageableResponseAsync(request, cancellationToken);
        }

        return query.OrderByDescending(request.OrderBy).ToCursorPageableResponseAsync(request, cancellationToken);
    }

    /// <summary>
    /// Orders a query with a key selector and converts it into a cursor-based pagination response.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the query.</typeparam>
    /// <typeparam name="TKey">The type of the key used for ordering.</typeparam>
    /// <param name="query">The queryable data source.</param>
    /// <param name="orderKeySelector">An expression that selects the key used for ordering.</param>
    /// <param name="request">The cursor request containing page size, cursor, and order direction.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="CursorPageableResponse{T}"/> containing page data and cursor metadata.</returns>
    public static Task<CursorPageableResponse<T>> ToCursorPageableResponseAsync<T, TKey>(
        this IQueryable<T> query,
        Expression<Func<T, TKey>> orderKeySelector,
        CursorPageableRequest request,
        CancellationToken cancellationToken)
    {
        ValidateCursorPageableRequest(request);
        ArgumentNullException.ThrowIfNull(orderKeySelector, nameof(orderKeySelector));

        if (request.OrderDirection == OrderDirectionEnum.Ascending)
        {
            return query.OrderBy(orderKeySelector).ToCursorPageableResponseAsync(request, cancellationToken);
        }

        return query.OrderByDescending(orderKeySelector).ToCursorPageableResponseAsync(request, cancellationToken);
    }

    /// <summary>
    /// Applies a greater-than or less-than filter based on cursor value and sort direction.
    /// </summary>
    private static IQueryable<T> ApplyCursorFilter<T>(IQueryable<T> query, string orderBy, object cursorValue, OrderDirectionEnum orderDirection)
    {
        ParameterExpression parameter = Expression.Parameter(typeof(T), "x");
        MemberExpression property = Expression.Property(parameter, orderBy);
        ConstantExpression constant = Expression.Constant(cursorValue);

        Expression convertedConstant = property.Type != cursorValue.GetType()
            ? Expression.Convert(constant, property.Type)
            : constant;

        BinaryExpression comparison = orderDirection == OrderDirectionEnum.Ascending
            ? Expression.GreaterThan(property, convertedConstant)
            : Expression.LessThan(property, convertedConstant);

        var lambda = Expression.Lambda<Func<T, bool>>(comparison, parameter);
        return query.Where(lambda);
    }

    /// <summary>
    /// Reads a property value from an instance by name.
    /// </summary>
    private static object GetPropertyValue<T>(T obj, string propertyName)
    {
        PropertyInfo? propertyInfo = typeof(T).GetProperty(propertyName);
        ArgumentNullException.ThrowIfNull(propertyInfo, $"Property '{propertyName}' does not exist on type '{typeof(T).Name}'.");
        return propertyInfo.GetValue(obj) ?? throw new InvalidOperationException($"Property '{propertyName}' value is null.");
    }

    /// <summary>
    /// Serializes and Base64-encodes a cursor value.
    /// </summary>
    private static string EncodeCursor(object value)
    {
        string json = JsonSerializer.Serialize(value);
        byte[] bytes = Encoding.UTF8.GetBytes(json);
        return Convert.ToBase64String(bytes);
    }

    /// <summary>
    /// Decodes and deserializes a Base64 cursor value.
    /// </summary>
    private static object DecodeCursor(string cursor)
    {
        try
        {
            byte[] bytes = Convert.FromBase64String(cursor);
            string json = Encoding.UTF8.GetString(bytes);

            using JsonDocument document = JsonDocument.Parse(json);
            JsonElement element = document.RootElement;

            return element.ValueKind switch
            {
                JsonValueKind.Number when element.TryGetInt32(out int intValue) => intValue,
                JsonValueKind.Number when element.TryGetInt64(out long longValue) => longValue,
                JsonValueKind.Number when element.TryGetDecimal(out decimal decimalValue) => decimalValue,
                JsonValueKind.Number when element.TryGetDouble(out double doubleValue) => doubleValue,
                JsonValueKind.String => element.GetString() ?? string.Empty,
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                _ => element.GetRawText()
            };
        }
        catch (Exception ex)
        {
            throw new ArgumentException("Invalid cursor format.", nameof(cursor), ex);
        }
    }

    /// <summary>
    /// Validates cursor request arguments.
    /// </summary>
    private static void ValidateCursorPageableRequest(CursorPageableRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(request.PageSize, 0, nameof(request.PageSize));
    }

}
