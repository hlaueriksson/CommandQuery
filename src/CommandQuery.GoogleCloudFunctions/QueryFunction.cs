using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryFunction"/> class.
        /// </summary>
        /// <param name="queryProcessor">An <see cref="IQueryProcessor"/>.</param>
        public QueryFunction(IQueryProcessor queryProcessor)
        {
            _queryProcessor = queryProcessor;
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
                    ? await HandleAsync(queryName, Dictionary(context.Request.Query)).ConfigureAwait(false)
                    : await HandleAsync(queryName, await context.Request.ReadAsStringAsync().ConfigureAwait(false)).ConfigureAwait(false);

                await context.Response.OkAsync(result).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                var payload = context.Request.Method == "GET" ? context.Request.QueryString.Value : await context.Request.ReadAsStringAsync().ConfigureAwait(false);
                logger?.LogError(exception, "Handle query failed: {Query}, {Payload}", queryName, payload);

                if (exception.IsHandled())
                {
                    await context.Response.BadRequestAsync(exception.ToError()).ConfigureAwait(false);
                    return;
                }

                await context.Response.InternalServerErrorAsync(exception.ToError()).ConfigureAwait(false);
            }

            static Dictionary<string, IEnumerable<string>> Dictionary(IQueryCollection query)
            {
                return query.ToDictionary(kv => kv.Key, kv => kv.Value as IEnumerable<string>, StringComparer.OrdinalIgnoreCase);
            }
        }

        private async Task<object> HandleAsync(string queryName, string content)
        {
            return await _queryProcessor.ProcessAsync<object>(queryName, content).ConfigureAwait(false);
        }

        private async Task<object> HandleAsync(string queryName, IDictionary<string, IEnumerable<string>> query)
        {
            return await _queryProcessor.ProcessAsync<object>(queryName, query).ConfigureAwait(false);
        }
    }
}
