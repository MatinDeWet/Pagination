using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Pagination.Enums;
using Pagination.Models.Requests;
using Pagination.Models.Responses;

namespace Pagination;
/// <summary>
/// Provides extension methods for offset-based pagination responses.
/// </summary>
public static class PageableExtensions
{
    /// <summary>
    /// Converts an already ordered query into an offset-based pagination response.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the query.</typeparam>
    /// <param name="query">The ordered queryable data source.</param>
    /// <param name="request">The paging request. <see cref="PageableRequest.PageNumber"/> and <see cref="BasePaginationRequest.PageSize"/> must be greater than 0.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A <see cref="PageableResponse{T}"/> that contains the requested page data and metadata.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the PageNumber or PageSize are less than or equal to 0.
    /// </exception>
    public static async Task<PageableResponse<T>> ToPageableResponseAsync<T>(this IOrderedQueryable<T> query, PageableRequest request, CancellationToken cancellationToken)
    {
        ValidatePageableRequest(request);

        int totalRecords = await query.CountAsync(cancellationToken)
            .ConfigureAwait(false);

        IQueryable<T> pageQuery = query.AsQueryable();

        int start = (request.PageNumber - 1) * request.PageSize;
        int pageCount = (int)Math.Ceiling(totalRecords / (double)request.PageSize);

        pageQuery = pageQuery.Skip(start);
        pageQuery = pageQuery.Take(request.PageSize);

        var result = new PageableResponse<T>
        {
            Data = await pageQuery.ToListAsync(cancellationToken).ConfigureAwait(false),
            PageSize = request.PageSize,
            PageNumber = request.PageNumber,
            PageCount = pageCount,
            TotalRecords = totalRecords,

            OrderDirection = request.OrderDirection,
            OrderBy = request.OrderBy ?? string.Empty,
        };

        return result;
    }

    /// <summary>
    /// Orders a query by <see cref="BasePaginationRequest.OrderBy"/> and converts it into an offset-based pagination response.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the query.</typeparam>
    /// <param name="query">The queryable data source.</param>
    /// <param name="request">The paging request. <see cref="BasePaginationRequest.OrderBy"/> must be provided.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A <see cref="PageableResponse{T}"/> that contains the requested page data and metadata.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the OrderBy property of the request is null, empty, or whitespace.
    /// </exception>
    public static Task<PageableResponse<T>> ToPageableResponseAsync<T>(this IQueryable<T> query, PageableRequest request, CancellationToken cancellationToken)
    {
        ValidatePageableRequest(request);

        ArgumentException.ThrowIfNullOrWhiteSpace(request.OrderBy, nameof(request.OrderBy));

        if (request.OrderDirection == OrderDirectionEnum.Ascending)
        {
            return query.OrderBy(request.OrderBy).ToPageableResponseAsync(request, cancellationToken);
        }
        else
        {
            return query.OrderByDescending(request.OrderBy).ToPageableResponseAsync(request, cancellationToken);
        }
    }

    /// <summary>
    /// Converts a query into an offset-based pagination response using either a request property name or a fallback key selector.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the query.</typeparam>
    /// <typeparam name="TKey">The type of the key used for ordering.</typeparam>
    /// <param name="query">The queryable data source.</param>
    /// <param name="orderKeySelector">A fallback selector used when <see cref="BasePaginationRequest.OrderBy"/> is not provided.</param>
    /// <param name="orderDirection">The direction used with <paramref name="orderKeySelector"/>.</param>
    /// <param name="request">The paging request containing page and optional ordering information.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A <see cref="PageableResponse{T}"/> that contains the requested page data and metadata.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the orderKeySelector is null when no OrderBy value is provided.
    /// </exception>
    public static Task<PageableResponse<T>> ToPageableResponseAsync<T, TKey>(
        this IQueryable<T> query,
        Expression<Func<T, TKey>> orderKeySelector,
        OrderDirectionEnum orderDirection,
        PageableRequest request,
        CancellationToken cancellationToken)
    {
        ValidatePageableRequest(request);

        if (!string.IsNullOrWhiteSpace(request.OrderBy))
        {
            return query.ToPageableResponseAsync(request, cancellationToken);
        }

        ArgumentNullException.ThrowIfNull(orderKeySelector, nameof(orderKeySelector));

        if (orderDirection == OrderDirectionEnum.Ascending)
        {
            return query.OrderBy(orderKeySelector).ToPageableResponseAsync(request, cancellationToken);
        }
        else
        {
            return query.OrderByDescending(orderKeySelector).ToPageableResponseAsync(request, cancellationToken);
        }
    }

    /// <summary>
    /// Backwards-compatible alias for <see cref="ToPageableResponseAsync{T}(IOrderedQueryable{T}, PageableRequest, CancellationToken)"/>.
    /// </summary>
    public static Task<PageableResponse<T>> ToPageableListAsync<T>(
        this IOrderedQueryable<T> query,
        PageableRequest request,
        CancellationToken cancellationToken)
    {
        return query.ToPageableResponseAsync(request, cancellationToken);
    }

    /// <summary>
    /// Backwards-compatible alias for <see cref="ToPageableResponseAsync{T}(IQueryable{T}, PageableRequest, CancellationToken)"/>.
    /// </summary>
    public static Task<PageableResponse<T>> ToPageableListAsync<T>(
        this IQueryable<T> query,
        PageableRequest request,
        CancellationToken cancellationToken)
    {
        return query.ToPageableResponseAsync(request, cancellationToken);
    }

    /// <summary>
    /// Backwards-compatible alias for <see cref="ToPageableResponseAsync{T, TKey}(IQueryable{T}, Expression{Func{T, TKey}}, OrderDirectionEnum, PageableRequest, CancellationToken)"/>.
    /// Uses <see cref="BasePaginationRequest.OrderDirection"/> from the request when no explicit direction is supplied.
    /// </summary>
    public static Task<PageableResponse<T>> ToPageableListAsync<T, TKey>(
        this IQueryable<T> query,
        Expression<Func<T, TKey>> orderKeySelector,
        PageableRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        return query.ToPageableResponseAsync(orderKeySelector, request.OrderDirection, request, cancellationToken);
    }

    private static void ValidatePageableRequest(PageableRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(request.PageNumber, 0, nameof(request.PageNumber));
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(request.PageSize, 0, nameof(request.PageSize));
    }

}
