using Microsoft.Extensions.DependencyInjection;
using System.Net;
using TodoList.Api.Entities;
using Xunit.Abstractions;

namespace TodoList.Api.IntegrationTests.TodoListEndpointTests;

public partial class GetTodoItemsId : IDisposable
{
    private TestGetWithInMemoryDB? _factory;
    private HttpClient? Client;

    private Guid guid = Guid.Parse("8223036d-ab15-4af7-8539-e532b3712156");

    public readonly ITestOutputHelper _testOutputHelper;

    public GetTodoItemsId(ITestOutputHelper testOutputHelper)
    {
        _factory = new TestGetWithInMemoryDB();

        var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TodoContext>();

        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        // Given - The database contains 3 items, one with a known I
        context.TodoItems.AddRange(
            new TodoItem { Id = guid, Description = "Item1" },
            new TodoItem { Id = Guid.NewGuid(), Description = "Item2" },
            new TodoItem { Id = Guid.NewGuid(), Description = "Item3" }
        );

        context.SaveChanges();

        Client = _factory.CreateClient();
        _testOutputHelper = testOutputHelper;
    }

    public void Dispose()
    {
        _factory?.Dispose();
        _factory = null;
        Client?.Dispose();
        Client = null;
    }

    [Theory]
    [InlineData("/api/todoitems/8223036d-ab15-4af7-8539-e532b3712156", HttpStatusCode.OK)]
    [InlineData("/api/todoitems/8223036d-ab15-4af7-8539-333333333333", HttpStatusCode.NotFound)]
    public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url, HttpStatusCode expectedStatusCode)
    {
        // When - A Put request is made to the url
        var response = await Client!.GetAsync(url);

        // Then - The expected Status Code is returned.
        response.StatusCode.ShouldBe(expectedStatusCode);
    }

    [Fact]
    public async Task Get_CheckTheShapeOf404Response()
    {
        // When - A Put request is made to the url
        var response = await Client!.GetAsync("/api/todoitems/8223036d-ab15-4af7-8539-333333333333");

        // Then - The body of the response should be the following JSON
        var content = await response.Content.ReadAsStringAsync();

        content.ShouldBeJsonEquivalent(new
        {
            type = "https://tools.ietf.org/html/rfc9110#section-15.5.5",
            title = "Not Found",
            status = 404,
        }, _testOutputHelper);
    }

    [Fact]
    public async Task Get_CheckTheShapeOf200Response()
    {
        // When - A Put request is made to the url
        var response = await Client!.GetAsync("/api/todoitems/8223036d-ab15-4af7-8539-e532b3712156");

        // Then - The body of the response should be the following JSON
        var content = await response.Content.ReadAsStringAsync();

        content.ShouldBeJsonEquivalent(new
        {
            id = "8223036d-ab15-4af7-8539-e532b3712156",
            description = "Item1",
            isCompleted = false
        }, _testOutputHelper);
    }
}


