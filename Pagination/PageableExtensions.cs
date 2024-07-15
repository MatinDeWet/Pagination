using Microsoft.EntityFrameworkCore;
using Pagination.Enums;
using Pagination.Helpers;
using Pagination.Models;
using System.Globalization;

namespace Pagination
{
    public static class PageableExtensions
    {
        public static async Task<PageableResponse<T>> ToPageableListAsync<T>(this IQueryable<T> query, PageableRequest request, CancellationToken cancellationToken)
        {
            int totalRecords = await query.CountAsync(cancellationToken);

            if (request.PageNumber > 0)
            {
                int start = (request.PageNumber - 1) * request.PageSize;
                query = query.Skip(start);
            }

            if (request.PageSize >= 0)
                query = query.Take(request.PageSize);

            int pageCount = (int)Math.Ceiling(totalRecords / (double)request.PageSize);

            var result = new PageableResponse<T>
            {
                Data = await query.ToListAsync(cancellationToken),
                PageSize = request.PageSize,
                PageNumber = request.PageNumber,
                PageCount = pageCount,
                TotalRecords = totalRecords
            };

            return result;
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
