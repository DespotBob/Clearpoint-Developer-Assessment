using Shouldly;
using System.Net;

namespace TodoList.Api.IntegrationTests.TodoListEndpointTests;

public class MiscellaneousGets
{
    private readonly TestGetWithInMemoryDB _factory;

    public MiscellaneousGets()
    {
        _factory = new TestGetWithInMemoryDB();
    }

    [Theory]
    [InlineData("/api/todoitems/", HttpStatusCode.OK)]
    public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url, HttpStatusCode expectedStatusCode)
    {
        // When - A Get request is made to the url
        var response = await _factory.CreateScopedClient().GetAsync(url);

        // Then - The expected Status Code is returned.
        response.StatusCode.ShouldBe(expectedStatusCode);
    }

    [Fact]
    public async Task Get_TodoItemsList()
    {
        // When - A Get request is made to the url
        var response = await _factory.CreateScopedClient().GetAsync("/api/todoitems");

        // Then - The expected Status Code is returned.
        response.ShouldSatisfyAllConditions(
            x => x.StatusCode.ShouldBe(HttpStatusCode.OK),
            x => x.Content.Headers.ContentType?.ToString().ShouldBe("application/json; charset=utf-8"));
    }
}
