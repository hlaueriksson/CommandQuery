using Amazon.Lambda.Annotations;
using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using CommandQuery.AWSLambda;

namespace CommandQuery.Sample.AWSLambda;

public class Command
{
    private readonly ICommandFunction _commandFunction;

    public Command(ICommandFunction commandFunction)
    {
        _commandFunction = commandFunction;
    }

    [LambdaFunction(Policies = "AWSLambdaBasicExecutionRole", MemorySize = 256, Timeout = 30)]
    [RestApi(LambdaHttpMethod.Post, "/command/{commandName}")]
    public async Task<APIGatewayProxyResponse> Handle(APIGatewayProxyRequest request, ILambdaContext context, string commandName) =>
        await _commandFunction.HandleAsync(commandName, request, context.Logger);
}
