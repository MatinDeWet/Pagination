namespace Pagination.Models.Responses;

/// <summary>
/// Represents an offset-based pagination response.
/// </summary>
/// <typeparam name="T">The type of the data elements.</typeparam>
public class PageableResponse<T> : BasePaginationResponse<T>
{
    /// <summary>
    /// Total number of records available before paging is applied.
    /// </summary>
    public int TotalRecords { get; set; }

    /// <summary>
    /// 1-based page number that was returned.
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// Total number of pages available.
    /// </summary>
    public int PageCount { get; set; }
}
