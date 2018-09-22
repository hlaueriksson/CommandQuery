using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using CommandQuery.AWSLambda.Internal;
using CommandQuery.DependencyInjection;
using CommandQuery.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace CommandQuery.AWSLambda
{
    public class QueryFunction
    {
        private readonly IQueryProcessor _queryProcessor;

        public QueryFunction(IQueryProcessor queryProcessor)
        {
            _queryProcessor = queryProcessor;
        }

        public async Task<object> Handle(string queryName, string content)
        {
            return await _queryProcessor.ProcessAsync<object>(queryName, content);
        }

        public async Task<object> Handle(string queryName, IDictionary<string, string> query)
        {
            return await _queryProcessor.ProcessAsync<object>(queryName, query);
        }

        public async Task<APIGatewayProxyResponse> Handle(string queryName, APIGatewayProxyRequest request, ILambdaContext context)
        {
            context.Logger.LogLine($"Handle {queryName}");

            try
            {
                var result = request.HttpMethod == "GET"
                    ? await Handle(queryName, request.QueryStringParameters)
                    : await Handle(queryName, request.Body);

                return new APIGatewayProxyResponse
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    Body = JsonConvert.SerializeObject(result),
                    Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                };
            }
            catch (QueryProcessorException exception)
            {
                context.Logger.LogLine("Handle query failed: " + exception);

                return exception.ToBadRequest();
            }
            catch (QueryValidationException exception)
            {
                context.Logger.LogLine("Handle query failed: " + exception);

                return exception.ToBadRequest();
            }
            catch (Exception exception)
            {
                context.Logger.LogLine("Handle query failed: " + exception);

                return exception.ToInternalServerError();
            }
        }
    }

    public static class QueryExtensions
    {
        public static IQueryProcessor GetQueryProcessor(this Assembly assembly)
        {
            return GetQueryProcessor(new ServiceCollection(), assembly);
        }

        public static IQueryProcessor GetQueryProcessor(this Assembly[] assemblies)
        {
            return GetQueryProcessor(new ServiceCollection(), assemblies);
        }

        public static IQueryProcessor GetQueryProcessor(this Assembly assembly, IServiceCollection services)
        {
            return GetQueryProcessor(services, assembly);
        }

        public static IQueryProcessor GetQueryProcessor(this Assembly[] assemblies, IServiceCollection services)
        {
            return GetQueryProcessor(services, assemblies);
        }

        private static IQueryProcessor GetQueryProcessor(IServiceCollection services, params Assembly[] assemblies)
        {
            services.AddQueries(assemblies);

            return new QueryProcessor(new QueryTypeCollection(assemblies), services.BuildServiceProvider());
        }
    }
}