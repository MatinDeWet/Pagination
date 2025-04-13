using Bogus;
using MockQueryable;
using Pagination.Enums;
using Pagination.UnitTests.Models;
using Shouldly;

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

    public class Pageabletests : IClassFixture<ClientFixture>
    {
        private readonly IQueryable<Client> _clients;

        public Pageabletests(ClientFixture fixture)
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
        public void OrderBy_ShouldOrderBySpecifiedProperty()
        {
            // Arrange
            var query = _clients;

            // Act
            var orderedQuery = query.OrderBy("FirstName");
            var list = orderedQuery.ToList();

            // Assert
            var expected = list.OrderBy(c => c.FirstName).ToList();
            list.ShouldBe(expected);
        }

        [Fact]
        public void OrderByDescending_ShouldOrderBySpecifiedPropertyDescending()
        {
            // Arrange
            var query = _clients;

            // Act
            var orderedQuery = query.OrderByDescending("FirstName");
            var list = orderedQuery.ToList();

            // Assert
            var expected = list.OrderByDescending(c => c.FirstName).ToList();
            list.ShouldBe(expected);
        }
    }
}
