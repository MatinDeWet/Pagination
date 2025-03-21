using Microsoft.EntityFrameworkCore;
using Pagination.Enums;
using Pagination.Models;
using System.Linq.Expressions;

namespace Pagination
{
    /// <summary>
    /// Provides extension methods for converting queryable sequences into pageable responses.
    /// </summary>
    public static class PageableExtensions
    {
        /// <summary>
        /// Asynchronously converts an ordered query into a pageable response based on the provided paging request.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the query.</typeparam>
        /// <param name="query">The ordered queryable data source.</param>
        /// <param name="request">
        /// The paging request containing the page number, page size, order by field, and order direction.
        /// PageNumber and PageSize must be greater than 0.
        /// </param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>
        /// A task representing the asynchronous operation, with a result of a <see cref="PageableResponse{T}"/>
        /// containing the paged data and metadata.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if the PageNumber or PageSize are less than or equal to 0.
        /// </exception>
        public static async Task<PageableResponse<T>> ToPageableListAsync<T>(this IOrderedQueryable<T> query, PageableRequest request, CancellationToken cancellationToken)
        {
            if (request.PageNumber <= 0)
                throw new ArgumentOutOfRangeException($"{nameof(request.PageNumber)} can not be less than or equal to 0");

            if (request.PageSize <= 0)
                throw new ArgumentOutOfRangeException($"{nameof(request.PageSize)} can not be less than or equal to 0");

            int totalRecords = await query.CountAsync(cancellationToken);

            IQueryable<T> pageQuery = query.AsQueryable();

            int start = (request.PageNumber - 1) * request.PageSize;
            int pageCount = (int)Math.Ceiling(totalRecords / (double)request.PageSize);

            pageQuery = pageQuery.Skip(start);
            pageQuery = pageQuery.Take(request.PageSize);

            var result = new PageableResponse<T>
            {
                Data = await pageQuery.ToListAsync(cancellationToken),
                PageSize = request.PageSize,
                PageNumber = request.PageNumber,
                PageCount = pageCount,
                TotalRecords = totalRecords,

                OrderDirection = request.OrderDirection,
                OrderBy = request.OrderBy,
            };

            return result;
        }

        /// <summary>
        /// Asynchronously converts a query into a pageable response by ordering the data using the specified OrderBy field.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the query.</typeparam>
        /// <param name="query">The queryable data source.</param>
        /// <param name="request">
        /// The paging request containing the page number, page size, order by field, and order direction.
        /// The OrderBy property must not be null, empty, or whitespace.
        /// </param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>
        /// A task representing the asynchronous operation, with a result of a <see cref="PageableResponse{T}"/>
        /// containing the paged data and metadata.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if the OrderBy property of the request is null, empty, or whitespace.
        /// </exception>
        public static async Task<PageableResponse<T>> ToPageableListAsync<T>(this IQueryable<T> query, PageableRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.OrderBy))
            {
                throw new ArgumentNullException(nameof(request.OrderBy));
            }

            if (request.OrderDirection == OrderDirectionEnum.Ascending)
                return await query.OrderBy(request.OrderBy).ToPageableListAsync(request, cancellationToken);
            else
                return await query.OrderByDescending(request.OrderBy).ToPageableListAsync(request, cancellationToken);
        }

        /// <summary>
        /// Asynchronously converts a query into a pageable response using a key selector for ordering when no explicit OrderBy is provided.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the query.</typeparam>
        /// <typeparam name="TKey">The type of the key used for ordering.</typeparam>
        /// <param name="query">The queryable data source.</param>
        /// <param name="orderKeySelector">
        /// An expression that selects the key used for ordering the data.
        /// This parameter is used only when the OrderBy property is not provided in the request.
        /// </param>
        /// <param name="request">
        /// The paging request containing the page number, page size, and order direction.
        /// </param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>
        /// A task representing the asynchronous operation, with a result of a <see cref="PageableResponse{T}"/>
        /// containing the paged data and metadata.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if the orderKeySelector is null when no OrderBy value is provided.
        /// </exception>
        public static async Task<PageableResponse<T>> ToPageableListAsync<T, TKey>(
            this IQueryable<T> query,
            Expression<Func<T, TKey>> orderKeySelector,
            PageableRequest request,
            CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(request.OrderBy))
                return await query.ToPageableListAsync(request, cancellationToken);

            if (orderKeySelector == null)
                throw new ArgumentNullException(nameof(orderKeySelector));

            if (request.OrderDirection == OrderDirectionEnum.Ascending)
                return await query.OrderBy(orderKeySelector).ToPageableListAsync(request, cancellationToken);
            else
                return await query.OrderByDescending(orderKeySelector).ToPageableListAsync(request, cancellationToken);
        }

        /// <summary>
        /// Orders the elements of a sequence in ascending order according to the specified property name.
        /// </summary>
        /// <typeparam name="T">The type of the elements of the source sequence.</typeparam>
        /// <param name="source">The queryable data source.</param>
        /// <param name="propertyName">The name of the property to use for ordering.</param>
        /// <returns>
        /// An <see cref="IOrderedQueryable{T}"/> whose elements are sorted in ascending order by the specified property.
        /// </returns>
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string propertyName)
        {
            return source.OrderBy(ToLambda<T>(propertyName));
        }

        /// <summary>
        /// Orders the elements of a sequence in descending order according to the specified property name.
        /// </summary>
        /// <typeparam name="T">The type of the elements of the source sequence.</typeparam>
        /// <param name="source">The queryable data source.</param>
        /// <param name="propertyName">The name of the property to use for ordering in descending order.</param>
        /// <returns>
        /// An <see cref="IOrderedQueryable{T}"/> whose elements are sorted in descending order by the specified property.
        /// </returns>
        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string propertyName)
        {
            return source.OrderByDescending(ToLambda<T>(propertyName));
        }

        /// <summary>
        /// Creates a lambda expression to access a specified property of an object.
        /// </summary>
        /// <typeparam name="T">The type of the object that contains the property.</typeparam>
        /// <param name="propertyName">The name of the property to access.</param>
        /// <returns>
        /// An expression representing a lambda that accesses the specified property, returning it as an object.
        /// </returns>
        private static Expression<Func<T, object>> ToLambda<T>(string propertyName)
        {
            var parameter = Expression.Parameter(typeof(T));
            var property = Expression.Property(parameter, propertyName);
            var propAsObject = Expression.Convert(property, typeof(object));

            return Expression.Lambda<Func<T, object>>(propAsObject, parameter);
        }
    }
}
