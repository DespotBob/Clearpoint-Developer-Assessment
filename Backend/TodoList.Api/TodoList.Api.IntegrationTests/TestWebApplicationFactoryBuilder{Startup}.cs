using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace TodoList.Api.IntegrationTests;

public abstract class AbstractTestWebApplicationFactoryBuilder : WebApplicationFactory<Startup>
{
    private HttpClient? _httpClient;
    private IServiceScope? _scope;

    public HttpClient CreateScopedClient()
    {
        if( _scope == null)
        {
            _scope = Services.CreateScope();
        }

        _httpClient = _httpClient ?? CreateClient();

        return _httpClient;
    }
}
