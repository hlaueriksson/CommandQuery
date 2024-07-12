# CommandQuery.AWSLambda ⚡

[![build](https://github.com/hlaueriksson/CommandQuery/actions/workflows/build.yml/badge.svg)](https://github.com/hlaueriksson/CommandQuery/actions/workflows/build.yml) [![CodeFactor](https://codefactor.io/repository/github/hlaueriksson/commandquery/badge)](https://codefactor.io/repository/github/hlaueriksson/commandquery)

> Command Query Separation for AWS Lambda

* Provides generic function support for commands and queries with *Amazon API Gateway*
* Enables APIs based on HTTP `POST` and `GET`

## Get Started

0. Install **AWS Toolkit for Visual Studio**
   * [https://aws.amazon.com/visualstudio/](https://aws.amazon.com/visualstudio/)
1. Create a new **AWS Serverless Application (.NET Core)** project
   * [Tutorial](https://docs.aws.amazon.com/toolkit-for-visual-studio/latest/user-guide/lambda-build-test-severless-app.html)
2. Install the `CommandQuery.AWSLambda` package from [NuGet](https://www.nuget.org/packages/CommandQuery.AWSLambda)
   * `PM>` `Install-Package CommandQuery.AWSLambda`
3. Create functions
   * Preferably named `Command` and `Query`
4. Create commands and command handlers
   * Implement `ICommand` and `ICommandHandler<in TCommand>`
   * Or `ICommand<TResult>` and `ICommandHandler<in TCommand, TResult>`
5. Create queries and query handlers
   * Implement `IQuery<TResult>` and `IQueryHandler<in TQuery, TResult>`
6. Configure the serverless template

![New Project - AWS Serverless Application (.NET Core - C#)](https://raw.githubusercontent.com/hlaueriksson/CommandQuery/master/vs-new-project-aws-serverless-application-1.png)

Choose:

* AWS Serverless Application (.NET Core - C#)

![New AWS Serverless Application - Empty Serverless Application](https://raw.githubusercontent.com/hlaueriksson/CommandQuery/master/vs-new-project-aws-serverless-application-2.png)

Choose:

* Empty Serverless Application

## Commands

```cs
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
```

* The function is requested via HTTP `POST` with the Content-Type `application/json` in the header.
* The name of the command is the slug of the URL.
* The command itself is provided as JSON in the body.
* If the command succeeds; the response is empty with the HTTP status code `200`.
* If the command fails; the response is an error message with the HTTP status code `400` or `500`.

Commands with result:

* If the command succeeds; the response is the result as JSON with the HTTP status code `200`.

## Queries

```cs
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
```

* The function is requested via:
  * HTTP `POST` with the Content-Type `application/json` in the header and the query itself as JSON in the body
  * HTTP `GET` and the query itself as query string parameters in the URL
* The name of the query is the slug of the URL.
* If the query succeeds; the response is the result as JSON with the HTTP status code `200`.
* If the query fails; the response is an error message with the HTTP status code `400` or `500`.

## Configuration

Configuration in `Startup.cs`:

```cs
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
```

The extension methods `AddCommandFunction` and `AddQueryFunction` will add functions and all command/query handlers in the given assemblies to the IoC container.
You can pass in a `params` array of `Assembly` arguments if your handlers are located in different projects.
If you only have one project you can use `typeof(Program).Assembly` as a single argument.

Configuration in `serverless.template`:

```json
{
  "AWSTemplateFormatVersion": "2010-09-09",
  "Transform": "AWS::Serverless-2016-10-31",
  "Description": "An AWS Serverless Application. This template is partially managed by Amazon.Lambda.Annotations (v1.5.0.0).",
  "Resources": {
    "CommandQuerySampleAWSLambdaCommandHandleGenerated": {
      "Type": "AWS::Serverless::Function",
      "Metadata": {
        "Tool": "Amazon.Lambda.Annotations",
        "SyncedEvents": [
          "RootPost"
        ],
        "SyncedEventProperties": {
          "RootPost": [
            "Path",
            "Method"
          ]
        }
      },
      "Properties": {
        "Architectures": [
          "x86_64"
        ],
        "Handler": "CommandQuery.Sample.AWSLambda::CommandQuery.Sample.AWSLambda.Command_Handle_Generated::Handle",
        "Runtime": "dotnet8",
        "CodeUri": ".",
        "MemorySize": 256,
        "Timeout": 30,
        "Policies": [
          "AWSLambdaBasicExecutionRole"
        ],
        "PackageType": "Zip",
        "Events": {
          "RootPost": {
            "Type": "Api",
            "Properties": {
              "Path": "/command/{commandName}",
              "Method": "POST"
            }
          }
        }
      }
    },
    "CommandQuerySampleAWSLambdaQueryGetGenerated": {
      "Type": "AWS::Serverless::Function",
      "Metadata": {
        "Tool": "Amazon.Lambda.Annotations",
        "SyncedEvents": [
          "RootGet"
        ],
        "SyncedEventProperties": {
          "RootGet": [
            "Path",
            "Method"
          ]
        }
      },
      "Properties": {
        "Architectures": [
          "x86_64"
        ],
        "Handler": "CommandQuery.Sample.AWSLambda::CommandQuery.Sample.AWSLambda.Query_Get_Generated::Get",
        "Runtime": "dotnet8",
        "CodeUri": ".",
        "MemorySize": 256,
        "Timeout": 30,
        "Policies": [
          "AWSLambdaBasicExecutionRole"
        ],
        "PackageType": "Zip",
        "Events": {
          "RootGet": {
            "Type": "Api",
            "Properties": {
              "Path": "/query/{queryName}",
              "Method": "GET"
            }
          }
        }
      }
    },
    "CommandQuerySampleAWSLambdaQueryPostGenerated": {
      "Type": "AWS::Serverless::Function",
      "Metadata": {
        "Tool": "Amazon.Lambda.Annotations",
        "SyncedEvents": [
          "RootPost"
        ],
        "SyncedEventProperties": {
          "RootPost": [
            "Path",
            "Method"
          ]
        }
      },
      "Properties": {
        "Architectures": [
          "x86_64"
        ],
        "Handler": "CommandQuery.Sample.AWSLambda::CommandQuery.Sample.AWSLambda.Query_Post_Generated::Post",
        "Runtime": "dotnet8",
        "CodeUri": ".",
        "MemorySize": 256,
        "Timeout": 30,
        "Policies": [
          "AWSLambdaBasicExecutionRole"
        ],
        "PackageType": "Zip",
        "Events": {
          "RootPost": {
            "Type": "Api",
            "Properties": {
              "Path": "/query/{queryName}",
              "Method": "POST"
            }
          }
        }
      }
    }
  },
  "Outputs": {
    "ApiURL": {
      "Description": "API endpoint URL for Prod environment",
      "Value": {
        "Fn::Sub": "https://${ServerlessRestApi}.execute-api.${AWS::Region}.amazonaws.com/Prod/"
      }
    }
  }
}
```

## Testing

You can [test](https://github.com/aws/aws-lambda-dotnet/blob/master/Libraries/src/Amazon.Lambda.TestUtilities/README.md) your lambdas with the [Amazon.Lambda.TestUtilities](https://www.nuget.org/packages/Amazon.Lambda.TestUtilities) package.

```cs
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;
using CommandQuery.AWSLambda;
using CommandQuery.Sample.Contracts.Queries;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CommandQuery.Sample.AWSLambda.Tests
{
    public class QueryTests
    {
        public class when_using_the_real_function_via_Post
        {
            [SetUp]
            public void SetUp()
            {
                var serviceCollection = new ServiceCollection();
                new Startup().ConfigureServices(serviceCollection);
                var serviceProvider = serviceCollection.BuildServiceProvider();

                Subject = new Query(serviceProvider.GetRequiredService<IQueryFunction>());
                Request = GetRequest("POST", content: "{ \"Id\": 1 }");
                Context = new TestLambdaContext();
            }

            [Test]
            public async Task should_work()
            {
                var result = await Subject.Post(Request, Context, "BarQuery");
                var value = result.As<Bar>()!;

                value.Id.Should().Be(1);
                value.Value.Should().NotBeEmpty();
            }

            [Test]
            public async Task should_handle_errors()
            {
                var result = await Subject.Post(Request, Context, "FailQuery");

                result.ShouldBeError("The query type 'FailQuery' could not be found");
            }

            Query Subject = null!;
            APIGatewayProxyRequest Request = null!;
            ILambdaContext Context = null!;
        }

        public class when_using_the_real_function_via_Get
        {
            [SetUp]
            public void SetUp()
            {
                var serviceCollection = new ServiceCollection();
                new Startup().ConfigureServices(serviceCollection);
                var serviceProvider = serviceCollection.BuildServiceProvider();

                Subject = new Query(serviceProvider.GetRequiredService<IQueryFunction>());
                Request = GetRequest("GET", query: new Dictionary<string, IList<string>> { { "Id", new List<string> { "1" } } });
                Context = new TestLambdaContext();
            }

            [Test]
            public async Task should_work()
            {
                var result = await Subject.Get(Request, Context, "BarQuery");
                var value = result.As<Bar>()!;

                value.Id.Should().Be(1);
                value.Value.Should().NotBeEmpty();
            }

            [Test]
            public async Task should_handle_errors()
            {
                var result = await Subject.Get(Request, Context, "FailQuery");

                result.ShouldBeError("The query type 'FailQuery' could not be found");
            }

            Query Subject = null!;
            APIGatewayProxyRequest Request = null!;
            ILambdaContext Context = null!;
        }

        static APIGatewayProxyRequest GetRequest(string method, string? content = null, Dictionary<string, IList<string>>? query = null)
        {
            var request = new APIGatewayProxyRequest
            {
                HttpMethod = method,
                Body = content,
                MultiValueQueryStringParameters = query
            };

            return request;
        }
    }
}
```

## Samples

* [CommandQuery.Sample.AWSLambda](https://github.com/hlaueriksson/CommandQuery/tree/master/samples/CommandQuery.Sample.AWSLambda)
* [CommandQuery.Sample.AWSLambda.Tests](https://github.com/hlaueriksson/CommandQuery/tree/master/samples/CommandQuery.Sample.AWSLambda.Tests)
