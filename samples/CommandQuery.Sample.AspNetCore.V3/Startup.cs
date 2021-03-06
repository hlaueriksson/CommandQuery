using CommandQuery.AspNetCore;
using CommandQuery.DependencyInjection;
using CommandQuery.Sample.Contracts.Commands;
using CommandQuery.Sample.Contracts.Queries;
using CommandQuery.Sample.Handlers;
using CommandQuery.Sample.Handlers.Commands;
using CommandQuery.Sample.Handlers.Queries;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CommandQuery.Sample.AspNetCore.V3
{
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
            services
                .AddControllers(options => options.Conventions.Add(new CommandQueryControllerModelConvention()))
                .ConfigureApplicationPartManager(manager =>
                {
                    manager.FeatureProviders.Add(new CommandControllerFeatureProvider(typeof(FooCommand).Assembly));
                    manager.FeatureProviders.Add(new QueryControllerFeatureProvider(typeof(BarQuery).Assembly));
                });

            // Add commands and queries
            services.AddCommands(typeof(FooCommandHandler).Assembly, typeof(FooCommand).Assembly);
            services.AddQueries(typeof(BarQueryHandler).Assembly, typeof(BarQuery).Assembly);

            // Add handler dependencies
            services.AddTransient<IDateTimeProxy, DateTimeProxy>();
            services.AddTransient<ICultureService, CultureService>();

            // Swagger
            services.AddOpenApiDocument(settings =>
            {
                settings.DocumentName = "v3";
                settings.Title = "CommandQuery.Sample.AspNetCore.V3";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Swagger
            app.UseOpenApi();
            app.UseSwaggerUi3();
            app.UseReDoc(settings =>
            {
                settings.Path = "/redoc";
                settings.DocumentPath = "/swagger/v3/swagger.json";
            });
        }
    }
}