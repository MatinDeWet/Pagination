using Pagination.Enums;
using Pagination.Models.Requests;
using Pagination.UnitTests.Models;
using Shouldly;

namespace Pagination.UnitTests.UnitTests;

public class PageableTests : IClassFixture<ClientFixture>
{
    private readonly IQueryable<Client> _clients;

    public PageableTests(ClientFixture fixture)
    {
        _clients = fixture.ClientsQueryable;
    }

    [Fact]
    public async Task ToPageableResponseAsync_WithOrderedQuery_ShouldReturnExpectedPage()
    {
        var request = new ClientPageableDto
        {
            PageNumber = 2,
            PageSize = 25,
            OrderDirection = OrderDirectionEnum.Ascending
        };

        var response = await _clients.OrderBy(c => c.Id).ToPageableResponseAsync(request, CancellationToken.None);

        response.TotalRecords.ShouldBe(1000);
        response.PageNumber.ShouldBe(2);
        response.PageSize.ShouldBe(25);
        response.PageCount.ShouldBe(40);
        response.OrderBy.ShouldBe(string.Empty);
        response.OrderDirection.ShouldBe(OrderDirectionEnum.Ascending);
        response.Data.Select(c => c.Id).ShouldBe(Enumerable.Range(26, 25));
    }

    [Fact]
    public async Task ToPageableResponseAsync_WithOrderByProperty_ShouldSortAscending()
    {
        var request = new ClientPageableDto
        {
            PageNumber = 1,
            PageSize = 20,
            OrderBy = "LastName",
            OrderDirection = OrderDirectionEnum.Ascending
        };

        var response = await _clients.ToPageableResponseAsync(request, CancellationToken.None);

        response.Data.ShouldBe(_clients.OrderBy(c => c.LastName).Take(20).ToList());
    }

    [Fact]
    public async Task ToPageableResponseAsync_WithOrderByPropertyDescending_ShouldSortDescending()
    {
        var request = new ClientPageableDto
        {
            PageNumber = 1,
            PageSize = 20,
            OrderBy = "LastName",
            OrderDirection = OrderDirectionEnum.Descending
        };

        var response = await _clients.ToPageableResponseAsync(request, CancellationToken.None);

        response.Data.ShouldBe(_clients.OrderByDescending(c => c.LastName).Take(20).ToList());
    }

    [Fact]
    public async Task ToPageableResponseAsync_WithKeySelector_ShouldUseFallbackWhenOrderByIsMissing()
    {
        var request = new ClientPageableDto
        {
            PageNumber = 3,
            PageSize = 15,
            OrderDirection = OrderDirectionEnum.Descending
        };

        var response = await _clients.ToPageableResponseAsync(c => c.Email, request.OrderDirection, request, CancellationToken.None);

        response.Data.ShouldBe(_clients.OrderByDescending(c => c.Email).Skip(30).Take(15).ToList());
    }

    [Fact]
    public async Task ToPageableListAsync_WithKeySelector_ShouldUseFallbackWhenOrderByIsMissing()
    {
        var request = new ClientPageableDto
        {
            PageNumber = 2,
            PageSize = 10,
            OrderDirection = OrderDirectionEnum.Ascending
        };

        var response = await _clients.ToPageableListAsync(c => c.LastName, request, CancellationToken.None);

        response.Data.ShouldBe(_clients.OrderBy(c => c.LastName).Skip(10).Take(10).ToList());
    }

    [Fact]
    public async Task ToPageableResponseAsync_WithExplicitOrderBy_ShouldIgnoreFallbackSelector()
    {
        var request = new ClientPageableDto
        {
            PageNumber = 1,
            PageSize = 10,
            OrderBy = "Id",
            OrderDirection = OrderDirectionEnum.Descending
        };

        var response = await _clients.ToPageableResponseAsync(c => c.Email, request.OrderDirection, request, CancellationToken.None);

        response.Data.ShouldBe(_clients.OrderByDescending(c => c.Id).Take(10).ToList());
    }

    [Fact]
    public async Task ToPageableResponseAsync_WhenRequestIsNull_ShouldThrowArgumentNullException()
    {
        Func<Task> act = async () =>
        {
            await _clients.OrderBy(c => c.Id).ToPageableResponseAsync(null!, CancellationToken.None);
        };

        var exception = await Should.ThrowAsync<ArgumentNullException>(act);
        exception.Message.ShouldContain("request");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task ToPageableResponseAsync_WhenPageNumberIsNotPositive_ShouldThrowArgumentOutOfRangeException(int pageNumber)
    {
        var request = new ClientPageableDto
        {
            PageNumber = pageNumber,
            PageSize = 10,
            OrderBy = "Id",
            OrderDirection = OrderDirectionEnum.Ascending
        };

        Func<Task> act = async () =>
        {
            await _clients.ToPageableResponseAsync(request, CancellationToken.None);
        };

        var exception = await Should.ThrowAsync<ArgumentOutOfRangeException>(act);
        exception.Message.ShouldContain("PageNumber");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task ToPageableResponseAsync_WhenPageSizeIsNotPositive_ShouldThrowArgumentOutOfRangeException(int pageSize)
    {
        var request = new ClientPageableDto
        {
            PageNumber = 1,
            PageSize = pageSize,
            OrderBy = "Id",
            OrderDirection = OrderDirectionEnum.Ascending
        };

        Func<Task> act = async () =>
        {
            await _clients.ToPageableResponseAsync(request, CancellationToken.None);
        };

        var exception = await Should.ThrowAsync<ArgumentOutOfRangeException>(act);
        exception.Message.ShouldContain("PageSize");
    }

    [Fact]
    public async Task ToPageableResponseAsync_WhenOrderByIsMissing_ShouldThrowArgumentNullException()
    {
        var request = new ClientPageableDto
        {
            PageNumber = 1,
            PageSize = 10,
            OrderBy = null,
            OrderDirection = OrderDirectionEnum.Ascending
        };

        Func<Task> act = async () =>
        {
            await _clients.ToPageableResponseAsync(request, CancellationToken.None);
        };

        var exception = await Should.ThrowAsync<ArgumentNullException>(act);
        exception.Message.ShouldContain("OrderBy");
    }

    [Theory]
    [InlineData(OrderDirectionEnum.Ascending)]
    [InlineData(OrderDirectionEnum.Descending)]
    public async Task ToPageableResponseAsync_WhenOrderByPropertyDoesNotExist_ShouldThrowArgumentException(OrderDirectionEnum orderDirection)
    {
        var request = new ClientPageableDto
        {
            PageNumber = 1,
            PageSize = 10,
            OrderBy = "NonExistentProperty",
            OrderDirection = orderDirection
        };

        Func<Task> act = async () =>
        {
            await _clients.ToPageableResponseAsync(request, CancellationToken.None);
        };

        var exception = await Should.ThrowAsync<ArgumentException>(act);
        exception.Message.ShouldContain("NonExistentProperty");
    }
}
