using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shouldly;
using System;
using System.Net;

namespace TodoList.Api.IntegrationTests;

public class TestWebApplicationFactoryBuilder : TestWebApplicationFactoryBuilder<Startup>, IDisposable
{
    public IServiceScope? Scope;

    Guid guid = Guid.Parse("8223036d-ab15-4af7-8539-e532b3712156");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.ConfigureServices(services =>
        {
            services.AddDbContext<TodoContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDbForTesting");
            });
        });
    }

    public HttpClient _httpClient; 

    public HttpClient GetClient()
    {
        if(_httpClient != null)
        {
            return _httpClient;
        }

        Scope = Services.CreateScope();

        var context = Scope.ServiceProvider.GetRequiredService<TodoContext>();

        // Given - The database contains 3 items, one with a known I 
        context.TodoItems.AddRange(
            new TodoItem { Id = guid, Description = "Item1" },
            new TodoItem { Id = Guid.NewGuid(), Description = "Item2" },
            new TodoItem { Id = Guid.NewGuid(), Description = "Item3" }
        );

        context.SaveChanges();

        _httpClient = CreateClient();  

        return _httpClient;
    }

    public override ValueTask DisposeAsync()
    {
        Scope?.Dispose();
        Scope = null;

        return base.DisposeAsync();
    }
}

public class TestSetEndpoints : IClassFixture<TestWebApplicationFactoryBuilder>
{
    private readonly TestWebApplicationFactoryBuilder _factory;

    private readonly HttpClient Client;


    TodoContext db;

    public TestSetEndpoints(TestWebApplicationFactoryBuilder factory)
    { 
        _factory = factory;
    }

    [Theory]
    [InlineData("/api/todoitems/8223036d-ab15-4af7-8539-e532b3712156", HttpStatusCode.OK)]
    [InlineData("/api/todoitems/8223036d-ab15-4af7-8539-333333333333", HttpStatusCode.NotFound)]
    public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url, HttpStatusCode expectedStatusCode)
    {
        // When - A Get request is made to the url
        var response = await _factory.GetClient().GetAsync(url);

        // Then - The expected Status Code is returned.
        response.StatusCode.ShouldBe(expectedStatusCode);
    }
}