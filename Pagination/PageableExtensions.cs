using Microsoft.EntityFrameworkCore;
using Pagination.Enums;
using Pagination.Helpers;
using Pagination.Models;
using System.Globalization;
using System.Linq.Expressions;

namespace Pagination
{
    public static class PageableExtensions
    {
        public static async Task<PageableResponse<T>> ToPageableListAsync<T>(this IOrderedQueryable<T> query, PageableRequest request, CancellationToken cancellationToken)
        {
            int totalRecords = await query.CountAsync(cancellationToken);

            IQueryable<T> pageQuery = query.AsQueryable();

            if (request.PageNumber > 0)
            {
                int start = (request.PageNumber - 1) * request.PageSize;
                pageQuery = pageQuery.Skip(start);
            }

            if (request.PageSize >= 0)
                pageQuery = pageQuery.Take(request.PageSize);

            int pageCount = (int)Math.Ceiling(totalRecords / (double)request.PageSize);

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

        public static Task<PageableResponse<T>> ToPageableListAsync<T>(this IQueryable<T> query, PageableRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.OrderBy))
            {
                throw new ArgumentNullException(nameof(request.OrderBy));
            }

            if (request.OrderDirection == OrderDirectionEnum.Ascending)
                return query.OrderBy(request.OrderBy).ToPageableListAsync(request, cancellationToken);
            else
                return query.OrderByDescending(request.OrderBy).ToPageableListAsync(request, cancellationToken);
        }

        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string propertyName)
        {
            return source.OrderBy(ToLambda<T>(propertyName));
        }

        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string propertyName)
        {
            return source.OrderByDescending(ToLambda<T>(propertyName));
        }

        private static Expression<Func<T, object>> ToLambda<T>(string propertyName)
        {
            var parameter = Expression.Parameter(typeof(T));
            var property = Expression.Property(parameter, propertyName);
            var propAsObject = Expression.Convert(property, typeof(object));

            return Expression.Lambda<Func<T, object>>(propAsObject, parameter);
        }

        public static string GetSearchPatternFormat(ExpressionMatchTypeEnum matchType)
        {
            return matchType switch
            {
                ExpressionMatchTypeEnum.StartsWith => "{0}%",
                ExpressionMatchTypeEnum.EndsWith => "%{0}",
                ExpressionMatchTypeEnum.Contains => "%{0}%",
                ExpressionMatchTypeEnum.Equals => "{0}",
                _ => throw new Exception($"Unknown match type {matchType}"),
            };
        }

        public static string BuildSearchPattern(this string term, ExpressionMatchTypeEnum matchType)
        {
            return string.Format(GetSearchPatternFormat(matchType), term);
        }

        public static string BuildSearchPattern(this string term, string patternFormat)
        {
            return string.Format(patternFormat, term);
        }

        public static IEnumerable<string> GetSearchTerms(this string searchTerms, bool toLower = false)
        {
            SqlCiStringComparer comparer = new SqlCiStringComparer();
            return ((from o in searchTerms?.Split(' ')
                     select o.Trim('%', '?', '(', ')') into t
                     select !toLower ? t : t.ToLower(CultureInfo.CurrentCulture) into t
                     where !string.IsNullOrWhiteSpace(t)
                     select t).Distinct(comparer) ?? Enumerable.Empty<string>()).ToList();
        }
    }
}
