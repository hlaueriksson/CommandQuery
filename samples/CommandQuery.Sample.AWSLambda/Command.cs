using Amazon.Lambda.Annotations;
using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using CommandQuery.AWSLambda;

namespace CommandQuery.Sample.AWSLambda;

public class Command(ICommandFunction commandFunction)
{
    [LambdaFunction]
    [RestApi(LambdaHttpMethod.Post, "/command/{commandName}")]
    public async Task<APIGatewayProxyResponse> Handle(APIGatewayProxyRequest request, ILambdaContext context, string commandName) =>
        await commandFunction.HandleAsync(commandName, request, context.Logger);
}
