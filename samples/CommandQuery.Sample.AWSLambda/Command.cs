using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using CommandQuery.AWSLambda;
using CommandQuery.Sample.Contracts.Commands;
using CommandQuery.Sample.Handlers;
using CommandQuery.Sample.Handlers.Commands;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CommandQuery.Sample.AWSLambda;

public class Command
{
    private readonly ICommandFunction _commandFunction;

    public Command()
    {
        var services = new ServiceCollection();
        //services.AddSingleton(new JsonSerializerOptions(JsonSerializerDefaults.Web));
        services.AddCommandFunction(typeof(FooCommandHandler).Assembly, typeof(FooCommand).Assembly);
        // Add handler dependencies
        services.AddTransient<ICultureService, CultureService>();

        var serviceProvider = services.BuildServiceProvider();
        serviceProvider.GetService<ICommandProcessor>().AssertConfigurationIsValid(); // Validation
        _commandFunction = serviceProvider.GetService<ICommandFunction>();
    }

    public async Task<APIGatewayProxyResponse> Handle(APIGatewayProxyRequest request, ILambdaContext context) =>
        await _commandFunction.HandleAsync(request.PathParameters["commandName"], request, context.Logger);
}
