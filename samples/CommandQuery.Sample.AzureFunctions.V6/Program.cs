using System.Text.Json;
using CommandQuery.AzureFunctions;
using CommandQuery.Sample.Contracts.Commands;
using CommandQuery.Sample.Contracts.Queries;
using CommandQuery.Sample.Handlers;
using CommandQuery.Sample.Handlers.Commands;
using CommandQuery.Sample.Handlers.Queries;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CommandQuery.Sample.AzureFunctions.V6
{
    public class Program
    {
        public static void Main()
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureServices(s =>
                {
                    //s.AddSingleton(new JsonSerializerOptions(JsonSerializerDefaults.Web));

                    // Add commands and queries
                    s.AddCommandFunction(typeof(FooCommandHandler).Assembly, typeof(FooCommand).Assembly);
                    s.AddQueryFunction(typeof(BarQueryHandler).Assembly, typeof(BarQuery).Assembly);

                    // Add handler dependencies
                    s.AddTransient<IDateTimeProxy, DateTimeProxy>();
                    s.AddTransient<ICultureService, CultureService>();
                })
                .Build();

            // Validation
            host.Services.GetService<ICommandProcessor>().AssertConfigurationIsValid();
            host.Services.GetService<IQueryProcessor>().AssertConfigurationIsValid();

            host.Run();
        }
    }
}
