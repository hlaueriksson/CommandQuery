using CommandQuery;
using CommandQuery.AzureFunctions;
using CommandQuery.Sample.Contracts.Commands;
using CommandQuery.Sample.Contracts.Queries;
using CommandQuery.Sample.Handlers;
using CommandQuery.Sample.Handlers.Commands;
using CommandQuery.Sample.Handlers.Queries;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(ConfigureServices)
    .Build();

// Validation
host.Services.GetService<ICommandProcessor>()!.AssertConfigurationIsValid();
host.Services.GetService<IQueryProcessor>()!.AssertConfigurationIsValid();

host.Run();

public static partial class Program
{
    public static void ConfigureServices(IServiceCollection services)
    {
        services
            //.AddSingleton(new JsonSerializerOptions(JsonSerializerDefaults.Web));

            // Add commands and queries
            .AddCommandFunction(typeof(FooCommandHandler).Assembly, typeof(FooCommand).Assembly)
            .AddQueryFunction(typeof(BarQueryHandler).Assembly, typeof(BarQuery).Assembly)

            // Add handler dependencies
            .AddTransient<IDateTimeProxy, DateTimeProxy>()
            .AddTransient<ICultureService, CultureService>();
    }
}
