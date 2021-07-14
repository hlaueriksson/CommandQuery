using System.Text.Json;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using CommandQuery.AWSLambda;
using CommandQuery.DependencyInjection;
using CommandQuery.Sample.Contracts.Queries;
using CommandQuery.Sample.Handlers;
using CommandQuery.Sample.Handlers.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace CommandQuery.Sample.AWSLambda
{
    public class Query
    {
        private static readonly QueryFunction _queryFunction = GetQueryFunction();

        public async Task<APIGatewayProxyResponse> Handle(APIGatewayProxyRequest request, ILambdaContext context)
        {
            return await _queryFunction.HandleAsync(request.PathParameters["queryName"], request, context.Logger);
        }

        private static QueryFunction GetQueryFunction()
        {
            var services = new ServiceCollection();
            // Add handler dependencies
            services.AddTransient<IDateTimeProxy, DateTimeProxy>();

            var queryProcessor = services.GetQueryProcessor(typeof(BarQueryHandler).Assembly, typeof(BarQuery).Assembly);
            // Validation
            queryProcessor.AssertConfigurationIsValid();

            //return new QueryFunction(queryProcessor, new JsonSerializerOptions(JsonSerializerDefaults.Web));
            return new QueryFunction(queryProcessor);
        }
    }
}
