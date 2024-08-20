namespace Pagination.Models
{
    public abstract class PageableSearchRequest : PageableRequest
    {
        /// <summary>
        /// Gets or sets the search terms.
        /// </summary>
        public virtual string SearchTerms { get; set; } = string.Empty!;

        public IEnumerable<string> GetSearchTerms(bool toLower = false)
        {
            return SearchTerms.GetSearchTerms(toLower);
        }
    }
}
