using System.Text.Json;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using CommandQuery.AWSLambda;
using CommandQuery.Sample.Contracts.Queries;
using CommandQuery.Sample.Handlers;
using CommandQuery.Sample.Handlers.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace CommandQuery.Sample.AWSLambda
{
    public class Query
    {
        private static readonly IQueryFunction _queryFunction = GetQueryFunction();

        public async Task<APIGatewayProxyResponse> Handle(APIGatewayProxyRequest request, ILambdaContext context)
        {
            return await _queryFunction.HandleAsync(request.PathParameters["queryName"], request, context.Logger);
        }

        private static IQueryFunction GetQueryFunction()
        {
            var services = new ServiceCollection();
            //services.AddSingleton(new JsonSerializerOptions(JsonSerializerDefaults.Web));
            services.AddQueryFunction(typeof(BarQueryHandler).Assembly, typeof(BarQuery).Assembly);
            // Add handler dependencies
            services.AddTransient<IDateTimeProxy, DateTimeProxy>();

            var serviceProvider = services.BuildServiceProvider();
            // Validation
            serviceProvider.GetService<IQueryProcessor>().AssertConfigurationIsValid();
            return serviceProvider.GetService<IQueryFunction>();
        }
    }
}
