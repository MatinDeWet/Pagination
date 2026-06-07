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
    public async Task ToCursorPageableResponseAsync_WhenFirstPage_ShouldReturnCursorMetadata()
    {
        var request = new CursorPageableRequest
        {
            PageSize = 100,
            OrderBy = "Id",
            OrderDirection = OrderDirectionEnum.Ascending,
            Cursor = null
        };

        var response = await _clients.ToCursorPageableResponseAsync(request, CancellationToken.None);

        response.Data.Count().ShouldBe(100);
        response.HasPreviousPage.ShouldBeFalse();
        response.HasNextPage.ShouldBeTrue();
        response.NextCursor.ShouldNotBeNullOrWhiteSpace();
        response.OrderBy.ShouldBe("Id");
        response.OrderDirection.ShouldBe(OrderDirectionEnum.Ascending);
        response.Data.First().Id.ShouldBe(1);
        response.Data.Last().Id.ShouldBe(100);
    }

    [Fact]
    public async Task ToCursorPageableResponseAsync_WhenNextPageRequested_ShouldAdvanceItems()
    {
        var firstRequest = new CursorPageableRequest
        {
            PageSize = 100,
            OrderBy = "Id",
            OrderDirection = OrderDirectionEnum.Ascending
        };

        var firstResponse = await _clients.ToCursorPageableResponseAsync(firstRequest, CancellationToken.None);

        var nextRequest = new CursorPageableRequest
        {
            PageSize = 100,
            OrderBy = "Id",
            OrderDirection = OrderDirectionEnum.Ascending,
            Cursor = firstResponse.NextCursor
        };

        var nextResponse = await _clients.ToCursorPageableResponseAsync(nextRequest, CancellationToken.None);

        nextResponse.HasPreviousPage.ShouldBeTrue();
        nextResponse.Data.First().Id.ShouldBe(101);
        nextResponse.Data.Last().Id.ShouldBe(200);
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

        Func<Task> act = async () => { await _clients.ToCursorPageableResponseAsync(request, CancellationToken.None); };

        var exception = await Should.ThrowAsync<ArgumentOutOfRangeException>(act);
        exception.Message.ShouldContain("PageSize");
    }

    [Fact]
    public async Task ToCursorPageableResponseAsync_WhenOrderByIsMissing_ShouldThrowArgumentNullException()
    {
        var request = new CursorPageableRequest
        {
            PageSize = 100,
            OrderBy = null,
            OrderDirection = OrderDirectionEnum.Ascending
        };

        Func<Task> act = async () => { await _clients.ToCursorPageableResponseAsync(request, CancellationToken.None); };

        var exception = await Should.ThrowAsync<ArgumentNullException>(act);
        exception.Message.ShouldContain("OrderBy");
    }
}
