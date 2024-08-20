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

        [Fact]
        public async Task ToPageableListAsync__ReturnsPageableResponse()
        {
            // Arrange
            var request = new ClientPageableDto
            {
                PageNumber = 1,
                PageSize = 10,
                OrderBy = "Id",
                OrderDirection = OrderDirectionEnum.Ascending
            };

            // Act
            var result = await _context.Clients.ToPageableListAsync(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task ToPageableListAsync__ForFirstPageTenResults__ReturnsPageableResponseWithCorrectDataCount()
        {
            // Arrange
            var request = new ClientPageableDto
            {
                PageNumber = 1,
                PageSize = 10,
                OrderBy = "Id",
                OrderDirection = OrderDirectionEnum.Ascending
            };

            // Act
            var result = await _context.Clients.ToPageableListAsync(request, CancellationToken.None);

            // Assert
            Assert.Equal(10, result.Data.Count());
        }

        [Fact]
        public async Task ToPageableListAsync__ForFirstPageTenResults__ReturnsPageableResponseWithCorrectTotalRecords()
        {
            // Arrange
            var request = new ClientPageableDto
            {
                PageNumber = 1,
                PageSize = 10,
                OrderBy = "Id",
                OrderDirection = OrderDirectionEnum.Ascending
            };

            // Act
            var result = await _context.Clients.ToPageableListAsync(request, CancellationToken.None);

            // Assert
            Assert.Equal(100, result.TotalRecords);
        }

        [Fact]
        public async Task ToPageableListAsync__ForFirstPageTenResults__ReturnsPageableResponseWithCorrectPageNumber()
        {
            // Arrange
            var request = new ClientPageableDto
            {
                PageNumber = 1,
                PageSize = 10,
                OrderBy = "Id",
                OrderDirection = OrderDirectionEnum.Ascending
            };

            // Act
            var result = await _context.Clients.ToPageableListAsync(request, CancellationToken.None);

            // Assert
            Assert.Equal(1, result.PageNumber);
        }

        [Fact]
        public async Task ToPageableListAsync__ForFirstPageTenResults__ReturnsPageableResponseWithCorrectPageSize()
        {
            // Arrange
            var request = new ClientPageableDto
            {
                PageNumber = 1,
                PageSize = 10,
                OrderBy = "Id",
                OrderDirection = OrderDirectionEnum.Ascending
            };

            // Act
            var result = await _context.Clients.ToPageableListAsync(request, CancellationToken.None);

            // Assert
            Assert.Equal(10, result.PageSize);
        }

        [Fact]
        public async Task ToPageableListAsync__ForFirstPageTenResults__ReturnsPageableResponseWithCorrectPageCount()
        {
            // Arrange
            var request = new ClientPageableDto
            {
                PageNumber = 1,
                PageSize = 10,
                OrderBy = "Id",
                OrderDirection = OrderDirectionEnum.Ascending
            };

            // Act
            var result = await _context.Clients.ToPageableListAsync(request, CancellationToken.None);

            // Assert
            Assert.Equal(10, result.PageCount);
        }

        [Fact]
        public async Task ToPageableListAsync__ForSecondPageTwentyResults__ReturnsPageableResponseWithCorrectDataCount()
        {
            // Arrange
            var request = new ClientPageableDto
            {
                PageNumber = 2,
                PageSize = 20,
                OrderBy = "Id",
                OrderDirection = OrderDirectionEnum.Ascending
            };

            // Act
            var result = await _context.Clients.ToPageableListAsync(request, CancellationToken.None);

            // Assert
            Assert.Equal(20, result.Data.Count());
        }

        [Fact]
        public async Task ToPageableListAsync__ForSecondPageTwentyResults__ReturnsPageableResponseWithCorrectTotalRecords()
        {
            // Arrange
            var request = new ClientPageableDto
            {
                PageNumber = 2,
                PageSize = 20,
                OrderBy = "Id",
                OrderDirection = OrderDirectionEnum.Ascending
            };

            // Act
            var result = await _context.Clients.ToPageableListAsync(request, CancellationToken.None);

            // Assert
            Assert.Equal(100, result.TotalRecords);
        }

        [Fact]
        public async Task ToPageableListAsync__ForSecondPageTwentyResults__ReturnsPageableResponseWithCorrectPageNumber()
        {
            // Arrange
            var request = new ClientPageableDto
            {
                PageNumber = 2,
                PageSize = 20,
                OrderBy = "Id",
                OrderDirection = OrderDirectionEnum.Ascending
            };

            // Act
            var result = await _context.Clients.ToPageableListAsync(request, CancellationToken.None);

            // Assert
            Assert.Equal(2, result.PageNumber);
        }

        [Fact]
        public async Task ToPageableListAsync__ForSecondPageTwentyResults__ReturnsPageableResponseWithCorrectPageSize()
        {
            // Arrange
            var request = new ClientPageableDto
            {
                PageNumber = 2,
                PageSize = 20,
                OrderBy = "Id",
                OrderDirection = OrderDirectionEnum.Ascending
            };

            // Act
            var result = await _context.Clients.ToPageableListAsync(request, CancellationToken.None);

            // Assert
            Assert.Equal(20, result.PageSize);
        }

        [Fact]
        public async Task ToPageableListAsync__ForSecondPageTwentyResults__ReturnsPageableResponseWithCorrectPageCount()
        {
            // Arrange
            var request = new ClientPageableDto
            {
                PageNumber = 2,
                PageSize = 20,
                OrderBy = "Id",
                OrderDirection = OrderDirectionEnum.Ascending
            };

            // Act
            var result = await _context.Clients.ToPageableListAsync(request, CancellationToken.None);

            // Assert
            Assert.Equal(5, result.PageCount);
        }

        [Fact]
        public async Task ToPageableListAsync__ForFirstPageThirtyResults__ReturnsPageableResponseWithCorrectDataCount()
        {
            // Arrange
            var request = new ClientPageableDto
            {
                PageNumber = 3,
                PageSize = 30,
                OrderBy = "Id",
                OrderDirection = OrderDirectionEnum.Ascending
            };

            // Act
            var result = await _context.Clients.ToPageableListAsync(request, CancellationToken.None);

            // Assert
            Assert.Equal(30, result.Data.Count());
        }

        [Fact]
        public async Task ToPageableListAsync__ForFirstPageThirtyResults__ReturnsPageableResponseWithCorrectTotalRecords()
        {
            // Arrange
            var request = new ClientPageableDto
            {
                PageNumber = 3,
                PageSize = 30,
                OrderBy = "Id",
                OrderDirection = OrderDirectionEnum.Ascending
            };

            // Act
            var result = await _context.Clients.ToPageableListAsync(request, CancellationToken.None);

            // Assert
            Assert.Equal(100, result.TotalRecords);
        }

        [Fact]
        public async Task ToPageableListAsync__ForFirstPageThirtyResults__ReturnsPageableResponseWithCorrectPageNumber()
        {
            // Arrange
            var request = new ClientPageableDto
            {
                PageNumber = 3,
                PageSize = 30,
                OrderBy = "Id",
                OrderDirection = OrderDirectionEnum.Ascending
            };

            // Act
            var result = await _context.Clients.ToPageableListAsync(request, CancellationToken.None);

            // Assert
            Assert.Equal(3, result.PageNumber);
        }

        [Fact]
        public async Task ToPageableListAsync__ForFirstPageThirtyResults__ReturnsPageableResponseWithCorrectPageSize()
        {
            // Arrange
            var request = new ClientPageableDto
            {
                PageNumber = 3,
                PageSize = 30,
                OrderBy = "Id",
                OrderDirection = OrderDirectionEnum.Ascending
            };

            // Act
            var result = await _context.Clients.ToPageableListAsync(request, CancellationToken.None);

            // Assert
            Assert.Equal(30, result.PageSize);
        }

        [Fact]
        public async Task ToPageableListAsync__ForFirstPageThirtyResults__ReturnsPageableResponseWithCorrectPageCount()
        {
            // Arrange
            var request = new ClientPageableDto
            {
                PageNumber = 3,
                PageSize = 30,
                OrderBy = "Id",
                OrderDirection = OrderDirectionEnum.Ascending
            };

            // Act
            var result = await _context.Clients.ToPageableListAsync(request, CancellationToken.None);

            // Assert
            Assert.Equal(4, result.PageCount);
        }

        [Fact]
        public async Task ToPageableListAsync__ForFithPageFiftyResults__ReturnsPageableResponseWithEmptyCollection()
        {
            // Arrange
            var request = new ClientPageableDto
            {
                PageNumber = 5,
                PageSize = 50,
                OrderBy = "Id",
                OrderDirection = OrderDirectionEnum.Ascending
            };

            // Act
            var result = await _context.Clients.ToPageableListAsync(request, CancellationToken.None);

            // Assert
            Assert.Empty(result.Data);
        }

        [Fact]
        public async Task ToPageableListAsync__ForFithPageFiftyResults__ReturnsPageableResponseWithCorrectTotalRecords()
        {
            // Arrange
            var request = new ClientPageableDto
            {
                PageNumber = 5,
                PageSize = 50,
                OrderBy = "Id",
                OrderDirection = OrderDirectionEnum.Ascending
            };

            // Act
            var result = await _context.Clients.ToPageableListAsync(request, CancellationToken.None);

            // Assert
            Assert.Equal(100, result.TotalRecords);
        }

        [Fact]
        public async Task ToPageableListAsync__ForFithPageFiftyResults__ReturnsPageableResponseWithCorrectPageNumber()
        {
            // Arrange
            var request = new ClientPageableDto
            {
                PageNumber = 5,
                PageSize = 50,
                OrderBy = "Id",
                OrderDirection = OrderDirectionEnum.Ascending
            };

            // Act
            var result = await _context.Clients.ToPageableListAsync(request, CancellationToken.None);

            // Assert
            Assert.Equal(5, result.PageNumber);
        }

        [Fact]
        public async Task ToPageableListAsync__ForFithPageFiftyResults__ReturnsPageableResponseWithCorrectPageSize()
        {
            // Arrange
            var request = new ClientPageableDto
            {
                PageNumber = 5,
                PageSize = 50,
                OrderBy = "Id",
                OrderDirection = OrderDirectionEnum.Ascending
            };

            // Act
            var result = await _context.Clients.ToPageableListAsync(request, CancellationToken.None);

            // Assert
            Assert.Equal(50, result.PageSize);
        }

        [Fact]
        public async Task ToPageableListAsync__ForFithPageFiftyResults__ReturnsPageableResponseWithCorrectPageCount()
        {
            // Arrange
            var request = new ClientPageableDto
            {
                PageNumber = 5,
                PageSize = 50,
                OrderBy = "Id",
                OrderDirection = OrderDirectionEnum.Ascending
            };

            // Act
            var result = await _context.Clients.ToPageableListAsync(request, CancellationToken.None);

            // Assert
            Assert.Equal(2, result.PageCount);
        }

        [Fact]
        public async Task ToPageableListAsync__ReturnsPageableResponseWithCorrect__OrderedAscendingData()
        {
            // Arrange
            var request = new ClientPageableDto
            {
                PageNumber = 1,
                PageSize = 10,
                OrderBy = "Id",
                OrderDirection = OrderDirectionEnum.Ascending
            };

            // Act
            var result = await _context.Clients.ToPageableListAsync(request, CancellationToken.None);

            // Assert
            var expected = _context.Clients.OrderBy(x => x.Id).Take(10).ToList();
            Assert.Equal(expected, result.Data);
        }

        [Fact]
        public async Task ToPageableListAsync__ReturnsPageableResponseWithCorrect__OrderedDescendingData()
        {
            // Arrange
            var request = new ClientPageableDto
            {
                PageNumber = 1,
                PageSize = 10,
                OrderBy = "Id",
                OrderDirection = OrderDirectionEnum.Descending
            };

            // Act
            var result = await _context.Clients.ToPageableListAsync(request, CancellationToken.None);

            // Assert
            var expected = _context.Clients.OrderByDescending(x => x.Id).Take(10).ToList();
            Assert.Equal(expected, result.Data);
        }

        [Fact]
        public async Task ToPageableListAsync__ThrowsArgumentNullException__WhenOrderByIsNullOrEmpty()
        {
            // Arrange
            var request = new ClientPageableDto
            {
                PageNumber = 1,
                PageSize = 10,
                OrderBy = string.Empty
            };

            // Act
            async Task Act() => await _context.Clients.ToPageableListAsync(request, CancellationToken.None);

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(Act);
        }
    }
}
