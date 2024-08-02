using System.Net;
using Xunit.Abstractions;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using TodoList.Api.Entities;

namespace TodoList.Api.IntegrationTests.TodoListEndpointTests;

public partial class PutPostTests : IDisposable
{
    public readonly ITestOutputHelper _testOutputHelper;
    private TestGetWithInMemoryDB? _factory;
    private HttpClient? Client;

    private Guid guid = Guid.Parse("8223036d-ab15-4af7-8539-e532b3712156");

    public PutPostTests(ITestOutputHelper testOutputHelper)
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
    public async Task Put_CanReplaceAnExistingEntity()
    {
        // When - A Put request is made to the url
        var response = await Client!.PutAsJsonAsync($"/api/todoitems/{guid}", new
        {
            id = guid,
            Description = "Item 1",
            isCompleted = true
        });

        // Then - The expected Status Code is returned.
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        // Then - The body of the response should be the following JSON
        var content = await response.Content.ReadAsStringAsync();

        content.ShouldBeJsonEquivalent(new
        {
            id = guid,
            description = "Item 1",
            isCompleted = true
        }, _testOutputHelper);
    }

    [Fact]
    public async Task Put_CannotCreateANewEntity()
    {
        var localGuid = new Guid("8223036d-ab15-4af7-8539-121212121212");

        // When - A Put request is made to the url
        var response = await Client!.PutAsJsonAsync($"/api/todoitems/{localGuid.ToString()}", new
        {
            id = localGuid.ToString(),
            Description = "Item 11",
            isCompleted = false
        });
        
        var content = await response.Content.ReadAsStringAsync();
        _testOutputHelper.WriteLine(content);

        // Then - The expected Status Code is returned.
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }


    [Fact]
    public async Task Put_ValidationStops()
    {
        var localGuid = new Guid("8223036d-ab15-4af7-8539-121212121212");

        // When - A Put request is made to the url
        var response = await Client!.PutAsJsonAsync($"/api/todoitems/{localGuid.ToString()}", new
        {
            id = localGuid.ToString(),
            Description = (string?) null,
            isCompleted = false
        });

        var content = await response.Content.ReadAsStringAsync();
        _testOutputHelper.WriteLine(content);

        // Then - The expected Status Code is returned.
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        // Then - The body of the response should be the following JSON

        // TODO: Check the shape of the 400 body.
    }


    [Fact]
    public async Task Post_DuplicateDescription()
    {
        // When - A Put request is made to the url
        var response = await Client!.PostAsJsonAsync("/api/todoitems/",
            new
            {
                id = "8223036d-ab15-4af7-8539-111111111111",
                description = "Item1",
                isCompleted = false
            });

        string content = await response.Content.ReadAsStringAsync();
        _testOutputHelper.WriteLine(content);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        content.ShouldBeJsonEquivalent(new
        {
            errors = new[]
            {
                new
                {
                    propertyName = "Description",
                    errorMessage = "A Todo item with that name already exists",
                }
            },
        }, _testOutputHelper);
    }

    [Fact]
    public async Task Post_DuplicateDescription_CheckForMissingValues()
    {
        // When - A Put request is made to the url
        var response = await Client!.PostAsJsonAsync("/api/todoitems/",
            new
            {
                id = "",
                description = "",
                isCompleted = false
            });

        string content = await response.Content.ReadAsStringAsync();
        _testOutputHelper.WriteLine(content);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        content.ShouldBeJsonEquivalent(new
        {
            errors = new[]
            { 
                new
                {
                    propertyName = "Description",
                    errorMessage = "'Description' must not be empty.",
                }
            },
        }, _testOutputHelper);

    }
}