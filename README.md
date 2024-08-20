# Pagination

[![CodeFactor](https://www.codefactor.io/repository/github/matindewet/pagination/badge)](https://www.codefactor.io/repository/github/matindewet/pagination)
[![NuGet Version](https://img.shields.io/nuget/v/MatinDeWet.Pagination)](https://www.nuget.org/packages/MatinDeWet.Pagination) 
[![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/MatinDeWet/Pagination/BuildTest.yml)](https://github.com/MatinDeWet/Pagination)

Pagination was designed to make it easy to paginate and order data in .NET applications making use of Entity Framework Core. 
It is a library that can be used to paginate any collection of data. It is designed to be easy to use and easy to extend.

## Usage
To use Pagination, you need to call the `Pagination` extension method on an `IQueryable<T>` object, 
you need to supply the method with a `PageableRequest` extended object and `cancellation token`. This will return a `PageableResponse<T>` object that contains the paginated data.

```C#
    var request = new ClientPageableDto : PageableRequest
    {
        PageNumber = 1,
        PageSize = 10,
        OrderBy = "Name",
        OrderDirection = OrderDirectionEnum.Ascending
    };

    var result = await _context.Clients.ToPageableListAsync(request, cancellationtoken);
```
You can specify the page number page size, OrderBy, and OrderDirection in the `PageableRequest` object. 
The `PageNumber` is the page you want to retrieve and the `PageSize` is the number of items you want to retrieve.
The `OrderBy` is the property you want to order by and the `OrderDirection` is the direction you want to order the data in.

The `PagedResult<T>` object contains the following:
```C#
    public class PageableResponse<T>
    {
        public IEnumerable<T> Data { get; set; } // The data for the current page

        public int TotalRecords { get; set; } // The total number of records in the collection

        public int PageNumber { get; set; } // The current page number

        public int PageSize { get; set; } // The number of items per page

        public int PageCount { get; set; } // The total number of pages

        public string OrderBy { get; set; } = string.Empty; // The property the data is ordered by

        public OrderDirectionEnum OrderDirection { get; set; } // The direction the data is ordered in
    }
```


The `PageableSearchRequest` allows you to apply searchterms to your object. 
You can create a method that accepts these searchterms seperated out into a IEnumerable of string. and allows a user to build a predicate to filter the data.
The Library [LinqKit.Microsoft.EntityFrameworkCore](https://www.nuget.org/packages/LinqKit.Microsoft.EntityFrameworkCore) can be used.

```C#
	public class ClientPageableSearchDto : PageableSearchRequest
	{
		public string SearchTerms { get; set; }
	}
```
```C#
    var request = new ClientPageableSearchDto : PageableSearchRequest
    {
        PageNumber = 1,
        PageSize = 10,
        SearchTerms = "Mat",
        OrderBy = "Name",
        OrderDirection = OrderDirectionEnum.Ascending
    };


    var result = await _context.Clients.ToPageableListAsync(request, cancellationtoken);
```

```C#
    internal static class ClientQueryBuilder
    {
        internal static IQueryable<Client> ApplyFilters(this IQueryable<Client> query, ClientPageableSearchDto request)
        {
            var searchPredicate = BuildSearchExpression(request.GetSearchTerms(true));
            return query
                .AsExpandable()
                .Where(searchPredicate);
        }

        internal static Expression<Func<Client, bool>> BuildSearchExpression(IEnumerable<string> terms, ExpressionMatchTypeEnum matchType = ExpressionMatchTypeEnum.StartsWith)
        {
            var predicate = PredicateBuilder.New<Client>(true);
            var patternFormat = SearchPatternBuilder.GetSearchPatternFormat(matchType);
            foreach (var term in terms)
            {
                var pattern = string.Format(patternFormat, term);

                predicate = predicate.And(o =>
                    EF.Functions.Like(o.Name.ToLower(), pattern)
                );
            }
            return predicate;
        }
    }
```

There is a extension method SearchPatternBuilder to help build patterns
```C#
    public static class SearchPatternBuilder
    {
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
    }
```