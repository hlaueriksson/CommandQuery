using Amazon.Lambda.Annotations;
using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using CommandQuery.AWSLambda;

namespace CommandQuery.Sample.AWSLambda
{
    public class Query
    {
        private readonly IQueryFunction _queryFunction;

        public Query(IQueryFunction queryFunction)
        {
            _queryFunction = queryFunction;
        }

        [LambdaFunction(Policies = "AWSLambdaBasicExecutionRole", MemorySize = 256, Timeout = 30)]
        [RestApi(LambdaHttpMethod.Get, "/query/{queryName}")]
        public async Task<APIGatewayProxyResponse> Get(APIGatewayProxyRequest request, ILambdaContext context, string queryName) =>
            await _queryFunction.HandleAsync(queryName, request, context.Logger);

        [LambdaFunction(Policies = "AWSLambdaBasicExecutionRole", MemorySize = 256, Timeout = 30)]
        [RestApi(LambdaHttpMethod.Post, "/query/{queryName}")]
        public async Task<APIGatewayProxyResponse> Post(APIGatewayProxyRequest request, ILambdaContext context, string queryName) =>
            await _queryFunction.HandleAsync(queryName, request, context.Logger);
    }
}
