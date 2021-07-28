#if NETCOREAPP3_1
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommandQuery.NewtonsoftJson;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CommandQuery.AzureFunctions
{
    /// <inheritdoc />
    public class QueryFunction : IQueryFunction
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly JsonSerializerSettings? _settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryFunction"/> class.
        /// </summary>
        /// <param name="queryProcessor">An <see cref="IQueryProcessor"/>.</param>
        /// <param name="settings"><see cref="JsonSerializerSettings"/> to control the behavior during deserialization of <see cref="HttpRequest.Body"/> and serialization of <see cref="HttpResponse.Body"/>.</param>
        public QueryFunction(IQueryProcessor queryProcessor, JsonSerializerSettings? settings = null)
        {
            _queryProcessor = queryProcessor;
            _settings = settings;
        }

        /// <inheritdoc />
        public async Task<IActionResult> HandleAsync(string queryName, HttpRequest req, ILogger? logger, CancellationToken cancellationToken = default)
        {
            logger?.LogInformation("Handle {Query}", queryName);

            if (req is null)
            {
                throw new ArgumentNullException(nameof(req));
            }

            try
            {
                var result = req.Method == "GET"
                    ? await _queryProcessor.ProcessAsync<object>(queryName, Dictionary(req.Query), cancellationToken).ConfigureAwait(false)
                    : await _queryProcessor.ProcessAsync<object>(queryName, await req.ReadAsStringAsync().ConfigureAwait(false), _settings, cancellationToken).ConfigureAwait(false);

                return result.Ok(_settings);
            }
            catch (Exception exception)
            {
                var payload = req.Method == "GET" ? req.QueryString.Value : await req.ReadAsStringAsync().ConfigureAwait(false);
                logger?.LogError(exception, "Handle query failed: {Query}, {Payload}", queryName, payload);

                return exception.IsHandled() ? exception.BadRequest(_settings) : exception.InternalServerError(_settings);
            }

            static Dictionary<string, IEnumerable<string>> Dictionary(IQueryCollection query)
            {
                return query.ToDictionary(kv => kv.Key, kv => kv.Value as IEnumerable<string>, StringComparer.OrdinalIgnoreCase);
            }
        }
    }
}
#endif
