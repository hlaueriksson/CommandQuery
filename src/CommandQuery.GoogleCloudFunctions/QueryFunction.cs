using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CommandQuery.SystemTextJson;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CommandQuery.GoogleCloudFunctions
{
    /// <inheritdoc />
    public class QueryFunction : IQueryFunction
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly JsonSerializerOptions? _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryFunction"/> class.
        /// </summary>
        /// <param name="queryProcessor">An <see cref="IQueryProcessor"/>.</param>
        /// <param name="options"><see cref="JsonSerializerOptions"/> to control the behavior during deserialization of <see cref="HttpRequest.Body"/> and serialization of <see cref="HttpResponse.Body"/>.</param>
        public QueryFunction(IQueryProcessor queryProcessor, JsonSerializerOptions? options = null)
        {
            _queryProcessor = queryProcessor;
            _options = options;
        }

        /// <inheritdoc />
        public async Task HandleAsync(string queryName, HttpContext context, ILogger? logger)
        {
            logger?.LogInformation("Handle {Query}", queryName);

            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            try
            {
                var result = context.Request.Method == "GET"
                    ? await _queryProcessor.ProcessAsync<object>(queryName, Dictionary(context.Request.Query)).ConfigureAwait(false)
                    : await _queryProcessor.ProcessAsync<object>(queryName, await context.Request.ReadAsStringAsync().ConfigureAwait(false), _options).ConfigureAwait(false);

                await context.Response.OkAsync(result, _options).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                var payload = context.Request.Method == "GET" ? context.Request.QueryString.Value : await context.Request.ReadAsStringAsync().ConfigureAwait(false);
                logger?.LogError(exception, "Handle query failed: {Query}, {Payload}", queryName, payload);

                if (exception.IsHandled())
                {
                    await context.Response.BadRequestAsync(exception).ConfigureAwait(false);
                    return;
                }

                await context.Response.InternalServerErrorAsync(exception).ConfigureAwait(false);
            }

            static Dictionary<string, IEnumerable<string>> Dictionary(IQueryCollection query)
            {
                return query.ToDictionary(kv => kv.Key, kv => kv.Value as IEnumerable<string>, StringComparer.OrdinalIgnoreCase);
            }
        }
    }
}
