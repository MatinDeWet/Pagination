namespace Pagination.Models
{
    public class PageableResponse<T>
    {
        public IEnumerable<T> Data { get; set; }

        public int TotalRecords { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int PageCount { get; set; }
    }
}
