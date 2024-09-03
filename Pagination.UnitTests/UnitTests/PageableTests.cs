using Pagination.Enums;
using Pagination.UnitTests.Context;
using Pagination.UnitTests.Models;

namespace Pagination.UnitTests.UnitTests
{
    public class PageableTests
    {
        private readonly TestContext _context;
        public PageableTests(TestContext context)
        {
            _context = context;
        }

        public static IEnumerable<object[]> GetPageableData =>
            new List<object[]>
            {
                new object[] { 1, 10, "Id", OrderDirectionEnum.Ascending },
                new object[] { 2, 20, "Id", OrderDirectionEnum.Ascending },
                new object[] { 3, 30, "Id", OrderDirectionEnum.Ascending },
                new object[] { 5, 50, "Id", OrderDirectionEnum.Ascending },
                new object[] { 10, 100, "Id", OrderDirectionEnum.Ascending },
            };

        [Theory]
        [MemberData(nameof(GetPageableData))]
        public void ToPageableList__ReturnsPageableResponse(int pageNumber, int pageSize, string orderBy, OrderDirectionEnum orderDirection)
        {
            // Arrange
            var request = new ClientPageableDto
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                OrderBy = orderBy,
                OrderDirection = orderDirection
            };

            // Act
            var result = _context.Clients.ToPageableListAsync(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
        }
    }
}
