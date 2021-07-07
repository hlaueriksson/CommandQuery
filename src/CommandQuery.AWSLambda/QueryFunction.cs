using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using CommandQuery.SystemTextJson;

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
        /// <param name="logger">An <see cref="ILambdaLogger"/>.</param>
        /// <returns>The result + 200, 400 or 500.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="request"/> is <see langword="null"/>.</exception>
        public async Task<APIGatewayProxyResponse> HandleAsync(string queryName, APIGatewayProxyRequest request, ILambdaLogger? logger)
        {
            logger?.LogLine($"Handle {queryName}");

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
                    Body = JsonSerializer.Serialize(result),
                    Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } },
                };
            }
            catch (Exception exception)
            {
                var payload = request.HttpMethod == "GET" ? request.Path : request.Body;
                logger?.LogLine($"Handle query failed: {queryName}, {payload}, {exception.Message}");

                return exception.IsHandled() ? exception.ToBadRequest() : exception.ToInternalServerError();
            }

            static Dictionary<string, IEnumerable<string>> Dictionary(IDictionary<string, IList<string>> query)
            {
                return query.ToDictionary(kv => kv.Key, kv => kv.Value as IEnumerable<string>, StringComparer.OrdinalIgnoreCase);
            }
        }
    }
}
