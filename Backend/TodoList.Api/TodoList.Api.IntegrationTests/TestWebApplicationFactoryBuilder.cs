using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

namespace TodoList.Api.IntegrationTests;

public class TestWebApplicationFactoryBuilder<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
{
    public IServiceProvider? ServiceProvider;

    protected override TestServer CreateServer(IWebHostBuilder builder)
    {
        var host = base.CreateServer(builder);

        ServiceProvider = host.Services;

        return host;
    }
}