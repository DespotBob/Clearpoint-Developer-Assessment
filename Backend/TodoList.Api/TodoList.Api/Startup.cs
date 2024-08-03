using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TodoList.Api.Entities;
using TodoList.Api.Middleware;

namespace TodoList.Api;


public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAllHeaders",
                  builder =>
                  {
                      builder.AllowAnyOrigin()
                             .AllowAnyHeader()
                             .AllowAnyMethod();
                  });
        });

        services.AddControllers();
        services.AddOpenApi();
        services.AddDbContext<TodoContext>(opt => opt.UseInMemoryDatabase("TodoItemsDB"));
        services.AddTransient<LocalExceptionHandlingMiddleware>();
        services.AddTransient<Repositories.ITodoRepository, Repositories.TodoRepository>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            // app.UseHttpsRedirection();  // Haven't sorted out certificates yet? So no compulsory HTTPS for now.
        }

        app.UseRouting();
        app.UseCors("AllowAllHeaders");
        app.UseMiddleware<LocalExceptionHandlingMiddleware>();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            // TODO: Convert Controllers to Minimal API... After figuring out 
            // if the openAPI documentation will still work..
            endpoints.MapControllers();
            endpoints.MapScalarUi();  // https://localhost:5001/scalar/v1
            endpoints.MapOpenApi();   // https://localhost:5001/openapi/v1.json
        });
    }
}