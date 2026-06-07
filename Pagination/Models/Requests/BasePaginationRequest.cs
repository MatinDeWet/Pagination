using Pagination.Enums;

namespace Pagination.Models.Requests;
/// <summary>
/// Base request for pagination options shared by offset and cursor flows.
/// </summary>
public abstract class BasePaginationRequest
{
    /// <summary>
    /// Number of records to return per page.
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Entity property name used for sorting.
    /// </summary>
    public string? OrderBy { get; set; }

    /// <summary>
    /// Sort direction. Defaults to <see cref="OrderDirectionEnum.Ascending"/>.
    /// </summary>
    public OrderDirectionEnum OrderDirection { get; set; } = OrderDirectionEnum.Ascending;
}
