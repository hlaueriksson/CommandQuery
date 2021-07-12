#if NET5_0
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using CommandQuery.SystemTextJson;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace CommandQuery.AzureFunctions
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
        public async Task<HttpResponseData> HandleAsync(string queryName, HttpRequestData req, ILogger? logger)
        {
            logger?.LogInformation("Handle {Query}", queryName);

            if (req is null)
            {
                throw new ArgumentNullException(nameof(req));
            }

            try
            {
                var result = req.Method == "GET"
                    ? await HandleAsync(queryName, Dictionary(req)).ConfigureAwait(false)
                    : await HandleAsync(queryName, await req.ReadAsStringAsync().ConfigureAwait(false)).ConfigureAwait(false);

                var response = req.CreateResponse();
                await response.WriteAsJsonAsync(result).ConfigureAwait(false);
                return response;
            }
            catch (Exception exception)
            {
                var payload = req.Method == "GET" ? req.Url.ToString() : await req.ReadAsStringAsync().ConfigureAwait(false);
                logger?.LogError(exception, "Handle query failed: {Query}, {Payload}", queryName, payload);

                return exception.IsHandled()
                    ? await req.BadRequestAsync(exception.ToError()).ConfigureAwait(false)
                    : await req.InternalServerErrorAsync(exception.ToError()).ConfigureAwait(false);
            }

            Dictionary<string, IEnumerable<string>> Dictionary(HttpRequestData req)
            {
                var query = HttpUtility.ParseQueryString(req.Url.Query);

                return query.AllKeys.ToDictionary<string?, string, IEnumerable<string>>(k => k!, k => query.GetValues(k)!);
            }
        }

        private async Task<object> HandleAsync(string queryName, string? content)
        {
            return await _queryProcessor.ProcessAsync<object>(queryName, content).ConfigureAwait(false);
        }

        private async Task<object> HandleAsync(string queryName, IDictionary<string, IEnumerable<string>> query)
        {
            return await _queryProcessor.ProcessAsync<object>(queryName, query).ConfigureAwait(false);
        }
    }
}
#endif
