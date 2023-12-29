using Amazon.Lambda.Annotations;
using Amazon.Lambda.Core;
using CommandQuery.AWSLambda;
using CommandQuery.Sample.Contracts.Commands;
using CommandQuery.Sample.Contracts.Queries;
using CommandQuery.Sample.Handlers;
using CommandQuery.Sample.Handlers.Commands;
using CommandQuery.Sample.Handlers.Queries;
using Microsoft.Extensions.DependencyInjection;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CommandQuery.Sample.AWSLambda;

[LambdaStartup]
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        //services.AddSingleton(new JsonSerializerOptions(JsonSerializerDefaults.Web));

        // Add commands and queries
        services.AddCommandFunction(typeof(FooCommandHandler).Assembly, typeof(FooCommand).Assembly);
        services.AddQueryFunction(typeof(BarQueryHandler).Assembly, typeof(BarQuery).Assembly);

        // Add handler dependencies
        services.AddTransient<IDateTimeProxy, DateTimeProxy>();
        services.AddTransient<ICultureService, CultureService>();

        // Validation
        var serviceProvider = services.BuildServiceProvider();
        serviceProvider.GetService<ICommandProcessor>()!.AssertConfigurationIsValid();
        serviceProvider.GetService<IQueryProcessor>()!.AssertConfigurationIsValid();
    }
}
