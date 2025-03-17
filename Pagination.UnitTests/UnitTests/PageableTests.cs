using Bogus;
using FluentAssertions;
using MockQueryable;
using Pagination.Enums;
using Pagination.UnitTests.Models;
using System.Linq.Expressions;

namespace Pagination.UnitTests.UnitTests
{
    public class ClientFixture
    {
        public IQueryable<Client> ClientsQueryable { get; }

        public ClientFixture()
        {
            var faker = new Faker<Client>()
                .RuleFor(c => c.Id, f => f.IndexFaker + 1)
                .RuleFor(c => c.FirstName, f => f.Name.FirstName())
                .RuleFor(c => c.LastName, f => f.Name.LastName())
                .RuleFor(c => c.Email, (f, c) => f.Internet.Email(c.FirstName, c.LastName));

            var clients = faker.Generate(1_000);

            ClientsQueryable = clients.AsQueryable().BuildMock();
        }
    }

    public class pageabletests : IClassFixture<ClientFixture>
    {
        private readonly IQueryable<Client> _clients;

        public pageabletests(ClientFixture fixture)
        {
            _clients = fixture.ClientsQueryable;
        }

        [Fact]
        public void ToPageableListAsync_WhenPageNumberIsLessThanZero_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange: request page -1 with pageSize 100.
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
            act.Should().ThrowAsync<ArgumentOutOfRangeException>()
                .WithMessage("*PageNumber*");
        }

        [Fact]
        public void ToPageableListAsync_WhenPageNumberIsZero_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange: request page 0 with pageSize 100.
            var request = new ClientPageableDto
            {
                PageNumber = 0,
                PageSize = 100,
                OrderBy = "Id",
                OrderDirection = OrderDirectionEnum.Ascending
            };
            CancellationToken cancellationToken = CancellationToken.None;

            // Act
            Func<Task> act = async () => { await _clients.ToPageableListAsync(request, cancellationToken); };

            // Assert
            act.Should().ThrowAsync<ArgumentOutOfRangeException>()
                .WithMessage("*PageNumber*");
        }

        [Fact]
        public async Task ToPageableListAsync_WhenPageNumberIsOne_ShouldReturnFirstPage()
        {
            // Arrange: request page 1 with pageSize 100.
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
            response.TotalRecords.Should().Be(1000);
            response.PageSize.Should().Be(100);
            response.PageNumber.Should().Be(1);
            response.PageCount.Should().Be((int)Math.Ceiling(1000 / 100.0));
            response.Data.Should().HaveCount(100);
            response.OrderBy.Should().Be("Id");
            response.OrderDirection.Should().Be(OrderDirectionEnum.Ascending);
            response.Data.First().Id.Should().Be(1);
        }

        [Fact]
        public async Task ToPageableListAsync_WhenPageNumberIsTwo_ShouldReturnSecondPage()
        {
            // Arrange: request page 2 with pageSize 100.
            var request = new ClientPageableDto
            {
                PageNumber = 2,
                PageSize = 100,
                OrderBy = "Id",
                OrderDirection = OrderDirectionEnum.Ascending
            };
            CancellationToken cancellationToken = CancellationToken.None;

            // Act
            var response = await _clients.ToPageableListAsync(request, cancellationToken);

            // Assert
            response.TotalRecords.Should().Be(1000);
            response.PageSize.Should().Be(100);
            response.PageNumber.Should().Be(2);
            response.PageCount.Should().Be((int)Math.Ceiling(1000 / 100.0));
            response.Data.Should().HaveCount(100);
            response.OrderBy.Should().Be("Id");
            response.OrderDirection.Should().Be(OrderDirectionEnum.Ascending);
            response.Data.First().Id.Should().Be(101);
        }

        [Fact]
        public void ToPageableListAsync_WhenPageSizeIsLessThanZero_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange: request page 1 with pageSize -1.
            var request = new ClientPageableDto
            {
                PageNumber = 1,
                PageSize = -1,
                OrderBy = "Id",
                OrderDirection = OrderDirectionEnum.Ascending
            };
            CancellationToken cancellationToken = CancellationToken.None;

            // Act
            Func<Task> act = async () => { await _clients.ToPageableListAsync(request, cancellationToken); };

            // Assert
            act.Should().ThrowAsync<ArgumentOutOfRangeException>()
                .WithMessage("*PageSize*");
        }

        [Fact]
        public void ToPageableListAsync_WhenPageSizeIsZero_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange: request all records by setting pageSize to 0.
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
            act.Should().ThrowAsync<ArgumentOutOfRangeException>()
                .WithMessage("*PageSize*");
        }

        [Fact]
        public async Task ToPageableListAsync_WhenPageSizeIsGreaterThanTotalRecords_ShouldReturnAllRecords()
        {
            // Arrange: request all records by setting pageSize to a value larger than total records.
            var request = new ClientPageableDto
            {
                PageNumber = 1,
                PageSize = 5000,
                OrderBy = "Id",
                OrderDirection = OrderDirectionEnum.Ascending
            };
            CancellationToken cancellationToken = CancellationToken.None;

            // Act
            var response = await _clients.ToPageableListAsync(request, cancellationToken);

            // Assert
            response.TotalRecords.Should().Be(1000);
            response.PageSize.Should().Be(5000);
            response.PageNumber.Should().Be(1);
            response.PageCount.Should().Be(1);
            response.Data.Should().HaveCount(1000);
        }

        [Fact]
        public async Task ToPageableListAsync_WhenPageSizeIsLessThanTotalRecords_ShouldReturnRequestedPage()
        {
            // Arrange: request page 2 with pageSize 100.
            var request = new ClientPageableDto
            {
                PageNumber = 2,
                PageSize = 100,
                OrderBy = "Id",
                OrderDirection = OrderDirectionEnum.Ascending
            };
            CancellationToken cancellationToken = CancellationToken.None;

            // Act
            var response = await _clients.ToPageableListAsync(request, cancellationToken);

            // Assert
            response.TotalRecords.Should().Be(1000);
            response.PageSize.Should().Be(100);
            response.PageNumber.Should().Be(2);
            response.PageCount.Should().Be((int)Math.Ceiling(1000 / 100.0));
            response.Data.Should().HaveCount(100);
            response.OrderBy.Should().Be("Id");
            response.OrderDirection.Should().Be(OrderDirectionEnum.Ascending);
            response.Data.First().Id.Should().Be(101);
        }

        [Fact]
        public async Task ToPageableListAsync_WhenOrderDirectionIsDescending_ShouldReturnDescendingOrder()
        {
            // Arrange: request page 1 with pageSize 100 and descending order.
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
            response.TotalRecords.Should().Be(1000);
            response.PageSize.Should().Be(100);
            response.PageNumber.Should().Be(1);
            response.PageCount.Should().Be((int)Math.Ceiling(1000 / 100.0));
            response.Data.Should().HaveCount(100);
            response.OrderBy.Should().Be("Id");
            response.OrderDirection.Should().Be(OrderDirectionEnum.Descending);
            response.Data.First().Id.Should().Be(1000);
        }
        [Fact]
        public async Task ToPageableListAsync_WhenOrderDirectionIsAscending_ShouldReturnAscendingOrder()
        {
            // Arrange: request page 1 with pageSize 100 and ascending order.
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
            response.TotalRecords.Should().Be(1000);
            response.PageSize.Should().Be(100);
            response.PageNumber.Should().Be(1);
            response.PageCount.Should().Be((int)Math.Ceiling(1000 / 100.0));
            response.Data.Should().HaveCount(100);
            response.OrderBy.Should().Be("Id");
            response.OrderDirection.Should().Be(OrderDirectionEnum.Ascending);
            response.Data.First().Id.Should().Be(1);
        }

        [Fact]
        public void ToPageableListAsync_WhenOrderByIsNotProvidedAndOrderKeySelectorNotProvided_ShouldThrowArgumentNullException()
        {
            // Arrange: request page 1 with pageSize 100 and no OrderBy.
            var request = new ClientPageableDto
            {
                PageNumber = 1,
                PageSize = 100,
                OrderBy = "",
                OrderDirection = OrderDirectionEnum.Ascending
            };
            CancellationToken cancellationToken = CancellationToken.None;

            // Act
            Func<Task> act = async () => { await _clients.ToPageableListAsync(request, cancellationToken); };

            // Assert
            act.Should().ThrowAsync<ArgumentNullException>()
                .WithMessage("*OrderBy*");
        }

        [Fact]
        public async Task ToPageableListAsync_WhenOrderByIsNotProvidedAndOrderKeySelectorIsProvided_ShouldOrderByKeySelector()
        {
            // Arrange: request page 1 with pageSize 100 and no OrderBy but keySelector is provided.
            var request = new ClientPageableDto
            {
                PageNumber = 1,
                PageSize = 100,
                OrderBy = "",
                OrderDirection = OrderDirectionEnum.Ascending
            };
            CancellationToken cancellationToken = CancellationToken.None;
            Expression<Func<Client, object>> keySelector = c => c.FirstName;

            // Act
            var response = await _clients.ToPageableListAsync(keySelector, request, cancellationToken);

            // Assert: verify that the data is ordered ascending by FirstName.
            var sortedData = response.Data.OrderBy(c => c.FirstName).ToList();
            response.Data.Should().Equal(sortedData);
        }

        [Fact]
        public void ToPageableListAsync_WhenKeySelectorIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange: request page 1 with pageSize 100 and no OrderBy but keySelector is null.
            var request = new ClientPageableDto
            {
                PageNumber = 1,
                PageSize = 100,
                OrderBy = "",
                OrderDirection = OrderDirectionEnum.Ascending
            };
            CancellationToken cancellationToken = CancellationToken.None;
            Expression<Func<Client, object>> keySelector = null!;

            // Act
            Func<Task> act = async () => { await _clients.ToPageableListAsync(keySelector, request, cancellationToken); };

            // Assert
            act.Should().ThrowAsync<ArgumentNullException>()
                .WithMessage("*keySelector*");
        }

        [Fact]
        public async Task ToPageableListAsync_WhenOrderByIsProvidedAndOrderKeySelectorIsProvided_ShouldOrderByOrderBy()
        {
            // Arrange: request page 1 with pageSize 100 and OrderBy and keySelector are provided.
            var request = new ClientPageableDto
            {
                PageNumber = 1,
                PageSize = 100,
                OrderBy = "Id",
                OrderDirection = OrderDirectionEnum.Ascending
            };
            CancellationToken cancellationToken = CancellationToken.None;
            Expression<Func<Client, object>> keySelector = c => c.FirstName;

            // Act
            var response = await _clients.ToPageableListAsync(keySelector, request, cancellationToken);

            // Assert: verify that the data is ordered ascending by Id.
            var sortedData = response.Data.OrderBy(c => c.Id).ToList();
            response.Data.Should().Equal(sortedData);
        }

        [Fact]
        public async Task ToPageableListAsync_WhenOrderByIsProvidedAndOrderKeyIsNotProvided_ShouldOrderByOrderBy()
        {
            // Arrange: request page 1 with pageSize 100 and OrderBy is provided but keySelector is not.
            var request = new ClientPageableDto
            {
                PageNumber = 1,
                PageSize = 100,
                OrderBy = "Id",
                OrderDirection = OrderDirectionEnum.Ascending
            };
            CancellationToken cancellationToken = CancellationToken.None;
            Expression<Func<Client, object>> keySelector = null!;

            // Act
            var response = await _clients.ToPageableListAsync(keySelector, request, cancellationToken);

            // Assert: verify that the data is ordered ascending by Id.
            var sortedData = response.Data.OrderBy(c => c.Id).ToList();
            response.Data.Should().Equal(sortedData);
        }

        [Fact]
        public void OrderBy_ShouldOrderBySpecifiedProperty()
        {
            // Arrange
            var query = _clients;

            // Act: order by "FirstName" using the extension.
            var orderedQuery = query.OrderBy("FirstName");
            var list = orderedQuery.ToList();

            // Assert: list should be sorted in ascending order by FirstName.
            var expected = list.OrderBy(c => c.FirstName).ToList();
            list.Should().Equal(expected);
        }

        [Fact]
        public void OrderByDescending_ShouldOrderBySpecifiedPropertyDescending()
        {
            // Arrange
            var query = _clients;

            // Act: order descending by "FirstName".
            var orderedQuery = query.OrderByDescending("FirstName");
            var list = orderedQuery.ToList();

            // Assert: list should be sorted in descending order by FirstName.
            var expected = list.OrderByDescending(c => c.FirstName).ToList();
            list.Should().Equal(expected);
        }
    }
}
