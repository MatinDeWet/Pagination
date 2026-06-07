namespace Pagination.Models.Requests;
/// <summary>
/// Represents a cursor-based pagination request.
/// </summary>
public class CursorPageableRequest : BasePaginationRequest
{
    /// <summary>
    /// Cursor token from the previous response. Leave null or empty to request the first page.
    /// </summary>
    public string? Cursor { get; set; }
}
