using Pagination.Enums;

namespace Pagination.Models
{
    public abstract class PageableRequest
    {
        /// <summary>
        /// Gets or sets the page number.
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Gets or sets the page size.
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// Gets or sets the order by. Refering to the property name of the entity.
        /// </summary>
        public string OrderBy { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the order direction. Default is ascending.
        /// </summary>
        public OrderDirectionEnum OrderDirection { get; set; } = OrderDirectionEnum.Ascending;
    }
}
