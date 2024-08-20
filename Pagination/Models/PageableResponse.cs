using Pagination.Enums;

namespace Pagination.Models
{
    public class PageableResponse<T>
    {
        /// <summary>
        /// Gets or sets the data. The collection of entities.
        /// </summary>
        public IEnumerable<T> Data { get; set; }

        /// <summary>
        /// Gets or sets the total records. The total number of records before pagination is applied.
        /// </summary>
        public int TotalRecords { get; set; }

        /// <summary>
        /// Gets or sets the page number. The Page Requested
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Gets or sets the page size. The number of records per page.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets the page count. The total number of pages. Calculated from TotalRecords and PageSize.
        /// </summary>
        public int PageCount { get; set; }

        /// <summary>
        /// Gets or sets the order by. The property name of the entity.
        /// </summary>
        public string OrderBy { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the order direction.
        /// </summary>
        public OrderDirectionEnum OrderDirection { get; set; }
    }
}
