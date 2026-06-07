namespace Pagination.Models.Responses;
/// <summary>
/// Represents a cursor-based pagination response.
/// </summary>
/// <typeparam name="T">The type of the data elements.</typeparam>
public class CursorPageableResponse<T> : BasePaginationResponse<T>
{
    /// <summary>
    /// Cursor to request the next page. Null means no next page is available.
    /// </summary>
    public string? NextCursor { get; set; }

    /// <summary>
    /// Cursor to request the previous page, when supported. Null typically means the first page.
    /// </summary>
    public string? PreviousCursor { get; set; }

    /// <summary>
    /// Indicates whether another page exists after this one.
    /// </summary>
    public bool HasNextPage { get; set; }

    /// <summary>
    /// Indicates whether a page exists before this one.
    /// </summary>
    public bool HasPreviousPage { get; set; }
}
