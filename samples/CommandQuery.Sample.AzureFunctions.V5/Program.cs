using System.Text.Json;
using CommandQuery.AzureFunctions;
using CommandQuery.DependencyInjection;
using CommandQuery.Sample.Contracts.Commands;
using CommandQuery.Sample.Contracts.Queries;
using CommandQuery.Sample.Handlers;
using CommandQuery.Sample.Handlers.Commands;
using CommandQuery.Sample.Handlers.Queries;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CommandQuery.Sample.AzureFunctions.V5
{
    public class Program
    {
        public static void Main()
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureServices(ConfigureServices)
                .Build();

            // Validation
            host.Services.GetService<ICommandProcessor>().AssertConfigurationIsValid();
            host.Services.GetService<IQueryProcessor>().AssertConfigurationIsValid();

            host.Run();
        }

        public static void ConfigureServices(IServiceCollection services)
        {
            services
                //.AddSingleton(new JsonSerializerOptions(JsonSerializerDefaults.Web));
                .AddSingleton<ICommandFunction, CommandFunction>()
                .AddSingleton<IQueryFunction, QueryFunction>()
                // Add commands and queries
                .AddCommands(typeof(FooCommandHandler).Assembly, typeof(FooCommand).Assembly)
                .AddQueries(typeof(BarQueryHandler).Assembly, typeof(BarQuery).Assembly)
                // Add handler dependencies
                .AddTransient<IDateTimeProxy, DateTimeProxy>()
                .AddTransient<ICultureService, CultureService>();
        }
    }
}
