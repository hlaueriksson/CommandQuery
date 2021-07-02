using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using CommandQuery.AWSLambda.Internal;
using CommandQuery.Internal;
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
        /// Initializes a new instance of the <see cref="QueryFunction"/> class.
        /// </summary>
        /// <param name="queryProcessor">An <see cref="IQueryProcessor"/>.</param>
        public QueryFunction(IQueryProcessor queryProcessor)
        {
            _queryProcessor = queryProcessor;
        }

        /// <summary>
        /// Handle a query.
        /// </summary>
        /// <param name="queryName">The name of the query.</param>
        /// <param name="request">An <see cref="APIGatewayProxyRequest"/>.</param>
        /// <param name="context">An <see cref="ILambdaContext"/>.</param>
        /// <returns>The result + 200, 400 or 500.</returns>
        public async Task<APIGatewayProxyResponse> HandleAsync(string queryName, APIGatewayProxyRequest request, ILambdaContext context)
        {
            context?.Logger.LogLine($"Handle {queryName}");

            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            try
            {
                var result = request.HttpMethod == "GET"
                    ? await _queryProcessor.ProcessAsync<object>(queryName, Dictionary(request.MultiValueQueryStringParameters)).ConfigureAwait(false)
                    : await _queryProcessor.ProcessAsync<object>(queryName, request.Body).ConfigureAwait(false);

                return new APIGatewayProxyResponse
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    Body = JsonConvert.SerializeObject(result),
                    Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } },
                };
            }
            catch (Exception exception)
            {
                var payload = request.HttpMethod == "GET" ? request.MultiValueQueryStringParameters.ToJson() : request.Body;
                context?.Logger.LogLine($"Handle query failed: {queryName}, {payload}, {exception.Message}");

                return exception.IsHandled() ? exception.ToBadRequest() : exception.ToInternalServerError();
            }

            Dictionary<string, IEnumerable<string>> Dictionary(IDictionary<string, IList<string>> query)
            {
                return query.ToDictionary(kv => kv.Key, kv => kv.Value as IEnumerable<string>, StringComparer.OrdinalIgnoreCase);
            }
        }
    }
}
