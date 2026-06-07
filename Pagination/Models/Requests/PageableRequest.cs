namespace Pagination.Models.Requests;
/// <summary>
/// Represents an offset-based pagination request.
/// </summary>
public abstract class PageableRequest : BasePaginationRequest
{
    /// <summary>
    /// 1-based page number to return.
    /// </summary>
    public int PageNumber { get; set; } = 1;
}
