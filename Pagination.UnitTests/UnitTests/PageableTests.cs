using MockQueryable;
using Pagination.Enums;
using Pagination.UnitTests.Models;
using Shouldly;

namespace Pagination.UnitTests.UnitTests
{
    public class PageableTests : IClassFixture<ClientFixture>
    {
        private readonly IQueryable<Client> _clients;

        public PageableTests(ClientFixture fixture)
        {
            _clients = fixture.ClientsQueryable;
        }

        [Fact]
        public async Task ToPageableListAsync_WhenPageNumberIsLessThanZero_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange
            var request = new ClientPageableDto
            {
                PageNumber = -1,
                PageSize = 100,
                OrderBy = "Id",
                OrderDirection = OrderDirectionEnum.Ascending
            };
            CancellationToken cancellationToken = CancellationToken.None;

            // Act
            Func<Task> act = async () => { await _clients.ToPageableListAsync(request, cancellationToken); };

            // Assert
            var exception = await Should.ThrowAsync<ArgumentOutOfRangeException>(act);
            exception.Message.ShouldContain("PageNumber");
        }

        [Fact]
        public async Task ToPageableListAsync_WhenPageNumberIsOne_ShouldReturnFirstPage()
        {
            // Arrange
            var request = new ClientPageableDto
            {
                PageNumber = 1,
                PageSize = 100,
                OrderBy = "Id",
                OrderDirection = OrderDirectionEnum.Ascending
            };
            CancellationToken cancellationToken = CancellationToken.None;

            // Act
            var response = await _clients.ToPageableListAsync(request, cancellationToken);

            // Assert
            response.TotalRecords.ShouldBe(1000);
            response.PageSize.ShouldBe(100);
            response.PageNumber.ShouldBe(1);
            response.PageCount.ShouldBe((int)Math.Ceiling(1000 / 100.0));
            response.Data.Count().ShouldBe(100);
            response.OrderBy.ShouldBe("Id");
            response.OrderDirection.ShouldBe(OrderDirectionEnum.Ascending);
            response.Data.First().Id.ShouldBe(1);
        }

        [Fact]
        public async Task ToPageableListAsync_ShouldOrderBySpecifiedProperty()
        {
            // Arrange
            var request = new ClientPageableDto
            {
                PageNumber = 1,
                PageSize = 100,
                OrderBy = "FirstName",
                OrderDirection = OrderDirectionEnum.Ascending
            };

            // Act
            var response = await _clients.ToPageableListAsync(request, CancellationToken.None);
            var list = response.Data.ToList();

            // Assert
            var expected = list.OrderBy(c => c.FirstName).ToList();
            list.ShouldBe(expected);
        }

        [Fact]
        public async Task ToPageableListAsync_ShouldOrderBySpecifiedPropertyDescending()
        {
            // Arrange
            var request = new ClientPageableDto
            {
                PageNumber = 1,
                PageSize = 100,
                OrderBy = "FirstName",
                OrderDirection = OrderDirectionEnum.Descending
            };

            // Act
            var response = await _clients.ToPageableListAsync(request, CancellationToken.None);
            var list = response.Data.ToList();

            // Assert
            var expected = list.OrderByDescending(c => c.FirstName).ToList();
            list.ShouldBe(expected);
        }

        [Fact]
        public async Task ToPageableListAsync_WhenPageSizeIsZero_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange
            var request = new ClientPageableDto
            {
                PageNumber = 1,
                PageSize = 0,
                OrderBy = "Id",
                OrderDirection = OrderDirectionEnum.Ascending
            };
            CancellationToken cancellationToken = CancellationToken.None;

            // Act
            Func<Task> act = async () => { await _clients.ToPageableListAsync(request, cancellationToken); };

            // Assert
            var exception = await Should.ThrowAsync<ArgumentOutOfRangeException>(act);
            exception.Message.ShouldContain("PageSize");
        }

        [Fact]
        public async Task ToPageableListAsync_WhenOrderByIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            var request = new ClientPageableDto
            {
                PageNumber = 1,
                PageSize = 100,
                OrderBy = null!,
                OrderDirection = OrderDirectionEnum.Ascending
            };
            CancellationToken cancellationToken = CancellationToken.None;

            // Act
            Func<Task> act = async () => { await _clients.ToPageableListAsync(request, cancellationToken); };

            // Assert
            var exception = await Should.ThrowAsync<ArgumentNullException>(act);
            exception.Message.ShouldContain("OrderBy");
        }

        [Fact]
        public async Task ToPageableListAsync_WhenOrderDirectionIsDescending_ShouldReturnDataInDescendingOrder()
        {
            // Arrange
            var request = new ClientPageableDto
            {
                PageNumber = 1,
                PageSize = 100,
                OrderBy = "Id",
                OrderDirection = OrderDirectionEnum.Descending
            };
            CancellationToken cancellationToken = CancellationToken.None;

            // Act
            var response = await _clients.ToPageableListAsync(request, cancellationToken);

            // Assert
            response.Data.First().Id.ShouldBe(1000);
            response.Data.Last().Id.ShouldBe(901);
        }

        [Fact]
        public async Task ToPageableListAsync_WithCustomOrderKeySelector_ShouldOrderByKeySelector()
        {
            // Arrange
            var request = new ClientPageableDto
            {
                PageNumber = 1,
                PageSize = 100,
                OrderBy = null!,
                OrderDirection = OrderDirectionEnum.Ascending
            };
            CancellationToken cancellationToken = CancellationToken.None;

            // Act
            var response = await _clients.ToPageableListAsync(c => c.LastName, request, cancellationToken);

            // Assert
            var expected = _clients.OrderBy(c => c.LastName).Take(100).ToList();
            response.Data.ShouldBe(expected);
        }

        [Fact]
        public async Task ToPageableListAsync_WhenOrderByPropertyDoesNotExist_ShouldThrowArgumentException()
        {
            // Arrange
            var request = new ClientPageableDto
            {
                PageNumber = 1,
                PageSize = 100,
                OrderBy = "NonExistentProperty",
                OrderDirection = OrderDirectionEnum.Ascending
            };

            // Act
            Func<Task> act = async () => { await _clients.ToPageableListAsync(request, CancellationToken.None); };

            // Assert
            var exception = await Should.ThrowAsync<ArgumentException>(act);
            exception.Message.ShouldContain("NonExistentProperty");
        }

        [Fact]
        public async Task ToPageableListAsync_WhenOrderByDescendingPropertyDoesNotExist_ShouldThrowArgumentException()
        {
            // Arrange
            var request = new ClientPageableDto
            {
                PageNumber = 1,
                PageSize = 100,
                OrderBy = "NonExistentProperty",
                OrderDirection = OrderDirectionEnum.Descending
            };

            // Act
            Func<Task> act = async () => { await _clients.ToPageableListAsync(request, CancellationToken.None); };

            // Assert
            var exception = await Should.ThrowAsync<ArgumentException>(act);
            exception.Message.ShouldContain("NonExistentProperty");
        }
    }
}
