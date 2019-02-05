using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using CommandQuery.AWSLambda.Internal;
using CommandQuery.Exceptions;
using Newtonsoft.Json;

namespace CommandQuery.AWSLambda
{
    /// <summary>
    /// Handles queries for the Lambda function.
    /// </summary>
    public class QueryFunction
    {
        private readonly IQueryProcessor _queryProcessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryFunction" /> class.
        /// </summary>
        /// <param name="queryProcessor">An <see cref="IQueryProcessor" /></param>
        public QueryFunction(IQueryProcessor queryProcessor)
        {
            _queryProcessor = queryProcessor;
        }

        /// <summary>
        /// Handle a query.
        /// </summary>
        /// <param name="queryName">The name of the query</param>
        /// <param name="content">The JSON representation of the query</param>
        /// <returns>The result of the query</returns>
        public async Task<object> Handle(string queryName, string content)
        {
            return await _queryProcessor.ProcessAsync<object>(queryName, content);
        }

        /// <summary>
        /// Handle a query.
        /// </summary>
        /// <param name="queryName">The name of the query</param>
        /// <param name="query">The key/value representation of the query</param>
        /// <returns>The result of the query</returns>
        public async Task<object> Handle(string queryName, IDictionary<string, string> query)
        {
            return await _queryProcessor.ProcessAsync<object>(queryName, query);
        }

        /// <summary>
        /// Handle a query.
        /// </summary>
        /// <param name="queryName">The name of the query</param>
        /// <param name="request">An <see cref="APIGatewayProxyRequest" /></param>
        /// <param name="context">An <see cref="ILambdaContext" /></param>
        /// <returns>The result + 200, 400 or 500</returns>
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
}