using Amazon.Lambda.Annotations;
using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using CommandQuery.AWSLambda;

namespace CommandQuery.Sample.AWSLambda
{
    public class Query(IQueryFunction queryFunction)
    {
        [LambdaFunction]
        [RestApi(LambdaHttpMethod.Get, "/query/{queryName}")]
        public async Task<APIGatewayProxyResponse> Get(APIGatewayProxyRequest request, ILambdaContext context, string queryName) =>
            await queryFunction.HandleAsync(queryName, request, context.Logger);

        [LambdaFunction]
        [RestApi(LambdaHttpMethod.Post, "/query/{queryName}")]
        public async Task<APIGatewayProxyResponse> Post(APIGatewayProxyRequest request, ILambdaContext context, string queryName) =>
            await queryFunction.HandleAsync(queryName, request, context.Logger);
    }
}
