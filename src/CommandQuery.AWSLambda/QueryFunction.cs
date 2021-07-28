using System;
using System.Collections.Generic;
using System.Linq;
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
    public class QueryFunction : IQueryFunction
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly JsonSerializerOptions? _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryFunction"/> class.
        /// </summary>
        /// <param name="queryProcessor">An <see cref="IQueryProcessor"/>.</param>
        /// <param name="options"><see cref="JsonSerializerOptions"/> to control the behavior during deserialization of <see cref="APIGatewayProxyRequest.Body"/> and serialization of <see cref="APIGatewayProxyResponse.Body"/>.</param>
        public QueryFunction(IQueryProcessor queryProcessor, JsonSerializerOptions? options = null)
        {
            _queryProcessor = queryProcessor;
            _options = options;
        }

        /// <inheritdoc />
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
                    : await _queryProcessor.ProcessAsync<object>(queryName, request.Body, _options).ConfigureAwait(false);

                return result.Ok(_options);
            }
            catch (Exception exception)
            {
                var payload = request.HttpMethod == "GET" ? request.Path : request.Body;
                logger?.LogLine($"Handle query failed: {queryName}, {payload}, {exception.Message}");

                return exception.IsHandled() ? exception.BadRequest(_options) : exception.InternalServerError(_options);
            }

            static Dictionary<string, IEnumerable<string>> Dictionary(IDictionary<string, IList<string>> query)
            {
                return query.ToDictionary(kv => kv.Key, kv => kv.Value as IEnumerable<string>, StringComparer.OrdinalIgnoreCase);
            }
        }
    }
}
