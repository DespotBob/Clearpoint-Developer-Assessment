using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace TodoList.Api.IntegrationTests;

public class TestGetWithInMemoryDB : AbstractTestWebApplicationFactoryBuilder
{
    private Guid guid = Guid.Parse("8223036d-ab15-4af7-8539-e532b3712156");

    private readonly string _dbName = $"InMemory-{Guid.NewGuid().ToString()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions>();
            services.RemoveAll<DbContextOptions<TodoContext>>();
            services.RemoveAll<TodoContext>();

            services
                .AddDbContext<TodoContext>((sp, options) =>
                {
                    options
                        .UseInMemoryDatabase(_dbName);

                   // ConfigureServices(sp);
                });
        });

        base.ConfigureWebHost(builder);
    }
}
