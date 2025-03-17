# Pagination

[![CodeFactor](https://www.codefactor.io/repository/github/matindewet/pagination/badge)](https://www.codefactor.io/repository/github/matindewet/pagination)
[![NuGet Version](https://img.shields.io/nuget/v/MatinDeWet.Pagination)](https://www.nuget.org/packages/MatinDeWet.Pagination) 
[![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/MatinDeWet/Pagination/BuildTest.yml)](https://github.com/MatinDeWet/Pagination)

Pagination is a .NET library designed to simplify the process of paginating and ordering data in applications using Entity Framework Core. It provides easy-to-use and extendable methods to paginate any collection of data.

## Features
- **Easy Pagination**: Paginate any `IQueryable<T>` collection with minimal code.
- **Dynamic Ordering**: Order data dynamically based on runtime parameters.
- **Asynchronous Execution**: Fully supports asynchronous operations with EF Core.

## Installation
You can install the Pagination library via NuGet: `dotnet add package MatinDeWet.Pagination`

## Usage
To use Pagination, call the `ToPageableListAsync` extension method on an `IQueryable<T>` object. Supply the method with a `PageableRequest` object and a `CancellationToken`. This will return a `PageableResponse<T>` object containing the paginated data.

### Basic Pagination Example
```C#
    var request = new ClientPageableDto : PageableRequest
    {
        Id = 1,
        Name = "John",
        PageNumber = 1,
        PageSize = 10,
        OrderBy = "Name",
        OrderDirection = OrderDirectionEnum.Ascending
    };

    var result = await _context.Clients.ToPageableListAsync(request, cancellationtoken);
```

### Ordering with a Key Selector
The `ToPageableListAsync<T, TKey>` method allows you to provide a key selector for ordering when the `OrderBy` property is not specified in the request. This serves as a fallback mechanism to ensure the data is still ordered appropriately.

#### Example
```C#
    var request = new ClientPageableDto : PageableRequest
    {
        Id = 1,
        Name = "John",
        PageNumber = 1,
        PageSize = 10,
        OrderBy = "Name",
        OrderDirection = OrderDirectionEnum.Ascending
    };

    var result = await _context.Clients.ToPageableListAsync(x => x.Id, request, cancellationtoken);
```

### PageableRequest Object
You can specify the `PageNumber`, `PageSize`, `OrderBy`, and `OrderDirection` in the `PageableRequest` object.
```C#
    public abstract class PageableRequest
    {
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public string OrderBy { get; set; } = string.Empty;

        public OrderDirectionEnum OrderDirection { get; set; } = OrderDirectionEnum.Ascending;
    }
```


### PageableResponse Object
The `PageableResponse<T>` object contains the following properties:
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