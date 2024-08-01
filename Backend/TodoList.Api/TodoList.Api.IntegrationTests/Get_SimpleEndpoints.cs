namespace TodoList.Api.IntegrationTests;

public class Get_SimpleEndpoints : IClassFixture<TestGetWithInMemoryDB>
{
    private readonly TestGetWithInMemoryDB _factory;

    private readonly HttpClient Client;

    public Get_SimpleEndpoints(TestGetWithInMemoryDB factory)
    {
        _factory = factory;

        // Given - A HttpClient to the SUT is created
        Client = _factory.CreateClient();
    }

    [Theory]
    [InlineData("/api/todoitems")]
    [InlineData("/swagger/v1/swagger.json")]
    public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
    {
        // When - A Get request is made to the url
        var response = await Client.GetAsync(url);

        // Then - A 200 OK status code is returned
        response.EnsureSuccessStatusCode();
    }
}