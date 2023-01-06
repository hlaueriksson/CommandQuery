using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using CommandQuery.AWSLambda;
using CommandQuery.Sample.Contracts.Queries;
using CommandQuery.Sample.Handlers;
using CommandQuery.Sample.Handlers.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace CommandQuery.Sample.AWSLambda
{
    public class Query
    {
        private readonly IQueryFunction _queryFunction;

        public Query()
        {
            var services = new ServiceCollection();
            //services.AddSingleton(new JsonSerializerOptions(JsonSerializerDefaults.Web));
            services.AddQueryFunction(typeof(BarQueryHandler).Assembly, typeof(BarQuery).Assembly);
            // Add handler dependencies
            services.AddTransient<IDateTimeProxy, DateTimeProxy>();

            var serviceProvider = services.BuildServiceProvider();
            serviceProvider.GetService<IQueryProcessor>().AssertConfigurationIsValid(); // Validation
            _queryFunction = serviceProvider.GetService<IQueryFunction>();
        }

        public async Task<APIGatewayProxyResponse> Handle(APIGatewayProxyRequest request, ILambdaContext context) =>
            await _queryFunction.HandleAsync(request.PathParameters["queryName"], request, context.Logger);
    }
}
