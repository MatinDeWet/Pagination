using Pagination.Enums;

namespace Pagination.Models.Responses;
/// <summary>
/// Base response shape shared by offset and cursor pagination results.
/// </summary>
/// <typeparam name="T">The type of the data elements.</typeparam>
public abstract class BasePaginationResponse<T>
{
    /// <summary>
    /// Items returned for the current page.
    /// </summary>
    public IEnumerable<T> Data { get; set; } = null!;

    /// <summary>
    /// Requested page size.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Property name used for sorting.
    /// </summary>
    public string OrderBy { get; set; } = string.Empty;

    /// <summary>
    /// Direction used for sorting.
    /// </summary>
    public OrderDirectionEnum OrderDirection { get; set; }
}
