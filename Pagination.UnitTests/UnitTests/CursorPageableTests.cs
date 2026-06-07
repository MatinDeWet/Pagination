using Pagination.Enums;
using Pagination.Models.Requests;
using Pagination.UnitTests.Models;
using Shouldly;

namespace Pagination.UnitTests.UnitTests;

public class CursorPageableTests : IClassFixture<ClientFixture>
{
    private readonly IQueryable<Client> _clients;

    public CursorPageableTests(ClientFixture fixture)
    {
        _clients = fixture.ClientsQueryable;
    }

    [Fact]
    public async Task ToCursorPageableResponseAsync_WithOrderedQuery_ShouldReturnFirstPageAndNextCursor()
    {
        var request = new CursorPageableRequest
        {
            PageSize = 25,
            OrderBy = "Id",
            OrderDirection = OrderDirectionEnum.Ascending
        };

        var response = await _clients.OrderBy(c => c.Id).ToCursorPageableResponseAsync(request, CancellationToken.None);

        response.Data.Select(c => c.Id).ShouldBe(Enumerable.Range(1, 25));
        response.HasPreviousPage.ShouldBeFalse();
        response.HasNextPage.ShouldBeTrue();
        response.NextCursor.ShouldNotBeNullOrWhiteSpace();
        response.PreviousCursor.ShouldBeNull();
        response.OrderBy.ShouldBe("Id");
        response.OrderDirection.ShouldBe(OrderDirectionEnum.Ascending);
    }

    [Fact]
    public async Task ToCursorPageableResponseAsync_WhenNextPageRequested_ShouldAdvanceItems()
    {
        var firstRequest = new CursorPageableRequest
        {
            PageSize = 25,
            OrderBy = "Id",
            OrderDirection = OrderDirectionEnum.Ascending
        };

        var firstResponse = await _clients.ToCursorPageableResponseAsync(firstRequest, CancellationToken.None);

        var nextRequest = new CursorPageableRequest
        {
            PageSize = 25,
            OrderBy = "Id",
            OrderDirection = OrderDirectionEnum.Ascending,
            Cursor = firstResponse.NextCursor
        };

        var nextResponse = await _clients.ToCursorPageableResponseAsync(nextRequest, CancellationToken.None);

        nextResponse.HasPreviousPage.ShouldBeTrue();
        nextResponse.PreviousCursor.ShouldBeNull();
        nextResponse.HasNextPage.ShouldBeTrue();
        nextResponse.Data.Select(c => c.Id).ShouldBe(Enumerable.Range(26, 25));
    }

    [Fact]
    public async Task ToCursorPageableResponseAsync_WithDescendingOrderBy_ShouldSortDescending()
    {
        var request = new CursorPageableRequest
        {
            PageSize = 20,
            OrderBy = "Id",
            OrderDirection = OrderDirectionEnum.Descending
        };

        var response = await _clients.ToCursorPageableResponseAsync(request, CancellationToken.None);

        response.Data.Select(c => c.Id).ShouldBe(Enumerable.Range(981, 20).Reverse());
        response.HasNextPage.ShouldBeTrue();
        response.HasPreviousPage.ShouldBeFalse();
        response.NextCursor.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task ToCursorPageableResponseAsync_WithKeySelector_ShouldUseFallbackWhenOrderByIsMissing()
    {
        var request = new CursorPageableRequest
        {
            PageSize = 1000,
            OrderDirection = OrderDirectionEnum.Descending
        };

        var response = await _clients.ToCursorPageableResponseAsync(c => c.Email, request, CancellationToken.None);

        response.HasNextPage.ShouldBeFalse();
        response.HasPreviousPage.ShouldBeFalse();
        response.NextCursor.ShouldBeNull();
        response.PreviousCursor.ShouldBeNull();
        response.Data.ShouldBe(_clients.OrderByDescending(c => c.Email).ToList());
    }

    [Fact]
    public async Task ToCursorPageableResponseAsync_WhenRequestIsNull_ShouldThrowArgumentNullException()
    {
        Func<Task> act = async () =>
        {
            await _clients.OrderBy(c => c.Id).ToCursorPageableResponseAsync(null!, CancellationToken.None);
        };

        var exception = await Should.ThrowAsync<ArgumentNullException>(act);
        exception.Message.ShouldContain("request");
    }

    [Fact]
    public async Task ToCursorPageableResponseAsync_WhenPageSizeIsZero_ShouldThrowArgumentOutOfRangeException()
    {
        var request = new CursorPageableRequest
        {
            PageSize = 0,
            OrderBy = "Id",
            OrderDirection = OrderDirectionEnum.Ascending
        };

        Func<Task> act = async () =>
        {
            await _clients.ToCursorPageableResponseAsync(request, CancellationToken.None);
        };

        var exception = await Should.ThrowAsync<ArgumentOutOfRangeException>(act);
        exception.Message.ShouldContain("PageSize");
    }

    [Fact]
    public async Task ToCursorPageableResponseAsync_WhenOrderByIsMissing_ShouldThrowArgumentNullException()
    {
        var request = new CursorPageableRequest
        {
            PageSize = 10,
            OrderBy = null,
            OrderDirection = OrderDirectionEnum.Ascending
        };

        Func<Task> act = async () =>
        {
            await _clients.ToCursorPageableResponseAsync(request, CancellationToken.None);
        };

        var exception = await Should.ThrowAsync<ArgumentNullException>(act);
        exception.Message.ShouldContain("OrderBy");
    }

    [Fact]
    public async Task ToCursorPageableResponseAsync_WhenOrderByPropertyDoesNotExist_ShouldThrowArgumentException()
    {
        var request = new CursorPageableRequest
        {
            PageSize = 10,
            OrderBy = "NonExistentProperty",
            OrderDirection = OrderDirectionEnum.Ascending
        };

        Func<Task> act = async () =>
        {
            await _clients.ToCursorPageableResponseAsync(request, CancellationToken.None);
        };

        var exception = await Should.ThrowAsync<ArgumentException>(act);
        exception.Message.ShouldContain("NonExistentProperty");
    }

    [Fact]
    public async Task ToCursorPageableResponseAsync_WhenCursorIsInvalid_ShouldThrowArgumentException()
    {
        var request = new CursorPageableRequest
        {
            PageSize = 10,
            OrderBy = "Id",
            OrderDirection = OrderDirectionEnum.Ascending,
            Cursor = "not-base64"
        };

        Func<Task> act = async () =>
        {
            await _clients.ToCursorPageableResponseAsync(request, CancellationToken.None);
        };

        var exception = await Should.ThrowAsync<ArgumentException>(act);
        exception.Message.ShouldContain("Invalid cursor format");
    }
}
