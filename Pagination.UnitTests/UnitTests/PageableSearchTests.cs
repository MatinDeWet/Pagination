using Pagination.UnitTests.Models;

namespace Pagination.UnitTests.UnitTests
{
    public class PageableSearchTests
    {
        [Fact]
        public void PageableSearchRequest_WhenSearchTextIsGiven_ShouldSeperateItIntoStringList()
        {
            // Arrange

            var pageable = new ClientPageableSearchDto
            {
                SearchTerms = "One Two Three"
            };

            // Act
            var result = pageable.GetSearchTerms();

            // Assert
            var items = new List<string> { "One", "Two", "Three" };
            Assert.Equal(items, result);
        }
    }
}
