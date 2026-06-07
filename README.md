# Pagination

[![CodeFactor](https://www.codefactor.io/repository/github/matindewet/pagination/badge)](https://www.codefactor.io/repository/github/matindewet/pagination)
[![NuGet Version](https://img.shields.io/nuget/v/MatinDeWet.Pagination)](https://www.nuget.org/packages/MatinDeWet.Pagination) 
[![CI Status](https://img.shields.io/github/actions/workflow/status/MatinDeWet/Pagination/CI.yml?branch=master)](https://github.com/MatinDeWet/Pagination/actions/workflows/CI.yml)
[![Publish Status](https://img.shields.io/github/actions/workflow/status/MatinDeWet/Pagination/nuget-publish.yml?branch=master)](https://github.com/MatinDeWet/Pagination/actions/workflows/nuget-publish.yml)

Pagination is a .NET library designed to simplify the process of paginating and ordering data in applications using Entity Framework Core. It provides easy-to-use and extendable methods to paginate any collection of data.

## Features
- **Offset Pagination**: Paginate any `IQueryable<T>` with page number and page size.
- **Cursor Pagination**: Page forward efficiently with opaque cursor tokens.
- **Dynamic Ordering**: Order data dynamically based on runtime parameters.
- **Asynchronous Execution**: Fully supports asynchronous operations with EF Core.

## Installation
You can install the Pagination library via NuGet: `dotnet add package MatinDeWet.Pagination`

## Usage
The library now exposes separate extension groups for offset pagination and cursor pagination:

- Offset pagination: `ToPageableResponseAsync(...)` (plus `ToPageableListAsync(...)` aliases for backward compatibility)
- Cursor pagination: `ToCursorPageableResponseAsync(...)`

### Offset Pagination

```csharp
public class ClientPageableDto : PageableRequest
{
    public string? Name { get; set; }
}

var request = new ClientPageableDto
{
    PageNumber = 1,
    PageSize = 10,
    OrderBy = "Name",
    OrderDirection = OrderDirectionEnum.Ascending
};

PageableResponse<Client> result =
    await _context.Clients.ToPageableResponseAsync(request, cancellationToken);
```

Fallback key selector when `OrderBy` is not supplied:

```csharp
PageableResponse<Client> result =
    await _context.Clients.ToPageableListAsync(c => c.Id, request, cancellationToken);
```

### Cursor Pagination

```csharp
var request = new CursorPageableRequest
{
    PageSize = 10,
    OrderBy = "Id",
    OrderDirection = OrderDirectionEnum.Ascending,
    Cursor = null
};

CursorPageableResponse<Client> firstPage =
    await _context.Clients.ToCursorPageableResponseAsync(request, cancellationToken);

var nextRequest = new CursorPageableRequest
{
    PageSize = 10,
    OrderBy = "Id",
    OrderDirection = OrderDirectionEnum.Ascending,
    Cursor = firstPage.NextCursor
};

CursorPageableResponse<Client> nextPage =
    await _context.Clients.ToCursorPageableResponseAsync(nextRequest, cancellationToken);
```

## Request Models

```csharp
public abstract class BasePaginationRequest
{
    public int PageSize { get; set; } = 10;
    public string? OrderBy { get; set; }
    public OrderDirectionEnum OrderDirection { get; set; } = OrderDirectionEnum.Ascending;
}

public abstract class PageableRequest : BasePaginationRequest
{
    public int PageNumber { get; set; } = 1;
}

public class CursorPageableRequest : BasePaginationRequest
{
    public string? Cursor { get; set; }
}
```

## Response Models

```csharp
public abstract class BasePaginationResponse<T>
{
    public IEnumerable<T> Data { get; set; } = null!;
    public int PageSize { get; set; }
    public string OrderBy { get; set; } = string.Empty;
    public OrderDirectionEnum OrderDirection { get; set; }
}

public class PageableResponse<T> : BasePaginationResponse<T>
{
    public int TotalRecords { get; set; }
    public int PageNumber { get; set; }
    public int PageCount { get; set; }
}

public class CursorPageableResponse<T> : BasePaginationResponse<T>
{
    public string? NextCursor { get; set; }
    public string? PreviousCursor { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
}
```