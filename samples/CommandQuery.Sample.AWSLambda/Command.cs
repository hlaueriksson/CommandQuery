using System.Text.Json;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using CommandQuery.AWSLambda;
using CommandQuery.Sample.Contracts.Commands;
using CommandQuery.Sample.Handlers;
using CommandQuery.Sample.Handlers.Commands;
using Microsoft.Extensions.DependencyInjection;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CommandQuery.Sample.AWSLambda
{
    public class Command
    {
        private static readonly ICommandFunction _commandFunction = GetCommandFunction();

        public async Task<APIGatewayProxyResponse> Handle(APIGatewayProxyRequest request, ILambdaContext context)
        {
            return await _commandFunction.HandleAsync(request.PathParameters["commandName"], request, context.Logger);
        }

        private static ICommandFunction GetCommandFunction()
        {
            var services = new ServiceCollection();
            //services.AddSingleton(new JsonSerializerOptions(JsonSerializerDefaults.Web));
            services.AddCommandFunction(typeof(FooCommandHandler).Assembly, typeof(FooCommand).Assembly);
            // Add handler dependencies
            services.AddTransient<ICultureService, CultureService>();

            var serviceProvider = services.BuildServiceProvider();
            // Validation
            serviceProvider.GetService<ICommandProcessor>().AssertConfigurationIsValid();
            return serviceProvider.GetService<ICommandFunction>();
        }
    }
}
