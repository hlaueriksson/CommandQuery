using System.Text.Json;
using CommandQuery.DependencyInjection;
using CommandQuery.GoogleCloudFunctions;
using CommandQuery.Sample.Contracts.Commands;
using CommandQuery.Sample.Contracts.Queries;
using CommandQuery.Sample.Handlers;
using CommandQuery.Sample.Handlers.Commands;
using CommandQuery.Sample.Handlers.Queries;
using Google.Cloud.Functions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace CommandQuery.Sample.GoogleCloudFunctions
{
    public class Startup : FunctionsStartup
    {
        public override void ConfigureServices(WebHostBuilderContext context, IServiceCollection services) =>
            services
                //.AddSingleton(new JsonSerializerOptions(JsonSerializerDefaults.Web))
                .AddSingleton<ICommandFunction, CommandFunction>()
                .AddSingleton<IQueryFunction, QueryFunction>()
                // Add commands and queries
                .AddCommands(typeof(FooCommandHandler).Assembly, typeof(FooCommand).Assembly)
                .AddQueries(typeof(BarQueryHandler).Assembly, typeof(BarQuery).Assembly)
                // Add handler dependencies
                .AddTransient<IDateTimeProxy, DateTimeProxy>()
                .AddTransient<ICultureService, CultureService>();

        public override void Configure(WebHostBuilderContext context, IApplicationBuilder app)
        {
            // Validation
            app.ApplicationServices.GetService<ICommandProcessor>().AssertConfigurationIsValid();
            app.ApplicationServices.GetService<IQueryProcessor>().AssertConfigurationIsValid();
        }
    }
}
