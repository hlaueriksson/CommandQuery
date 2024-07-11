using System.Text.Json;
using System.Web;
using CommandQuery.SystemTextJson;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace CommandQuery.AzureFunctions
{
    /// <inheritdoc />
    public class QueryFunction : IQueryFunction
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly ILogger<QueryFunction> _logger;
        private readonly JsonSerializerOptions? _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryFunction"/> class.
        /// </summary>
        /// <param name="queryProcessor">An <see cref="IQueryProcessor"/>.</param>
        /// <param name="logger">An <see cref="ILogger{T}"/>.</param>
        /// <param name="options"><see cref="JsonSerializerOptions"/> to control the behavior during deserialization of <see cref="HttpRequestData.Body"/> and serialization of <see cref="HttpResponseData.Body"/>.</param>
        public QueryFunction(IQueryProcessor queryProcessor, ILogger<QueryFunction> logger, JsonSerializerOptions? options = null)
        {
            _queryProcessor = queryProcessor;
            _logger = logger;
            _options = options;
        }

        /// <inheritdoc />
        public async Task<HttpResponseData> HandleAsync(string queryName, HttpRequestData req, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(req);

            _logger.LogInformation("Handle {Query}", queryName);

            try
            {
                var result = req.Method == "GET"
                    ? await _queryProcessor.ProcessAsync<object>(queryName, Dictionary(req.Url), cancellationToken).ConfigureAwait(false)
                    : await _queryProcessor.ProcessAsync<object>(queryName, await req.ReadAsStringAsync().ConfigureAwait(false), _options, cancellationToken).ConfigureAwait(false);

                return await req.OkAsync(result, _options).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                var payload = req.Method == "GET" ? req.Url.ToString() : await req.ReadAsStringAsync().ConfigureAwait(false);
                _logger.LogError(exception, "Handle query failed: {Query}, {Payload}", queryName, payload);

                return exception.IsHandled()
                    ? await req.BadRequestAsync(exception, _options).ConfigureAwait(false)
                    : await req.InternalServerErrorAsync(exception, _options).ConfigureAwait(false);
            }

            Dictionary<string, IEnumerable<string>> Dictionary(Uri url)
            {
                var query = HttpUtility.ParseQueryString(url.Query);

                return query.AllKeys.ToDictionary<string?, string, IEnumerable<string>>(k => k!, k => query.GetValues(k)!);
            }
        }

        /// <inheritdoc />
        public async Task<IActionResult> HandleAsync(string queryName, HttpRequest req, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(req);

            _logger.LogInformation("Handle {Query}", queryName);

            try
            {
                var result = req.Method == "GET"
                    ? await _queryProcessor.ProcessAsync<object>(queryName, Dictionary(req.Query), cancellationToken).ConfigureAwait(false)
                    : await _queryProcessor.ProcessAsync<object>(queryName, await req.ReadAsStringAsync().ConfigureAwait(false), _options, cancellationToken).ConfigureAwait(false);

                return new OkObjectResult(result);
            }
            catch (Exception exception)
            {
                var payload = req.Method == "GET" ? req.GetDisplayUrl() : await req.ReadAsStringAsync().ConfigureAwait(false);
                _logger.LogError(exception, "Handle query failed: {Query}, {Payload}", queryName, payload);

                return exception.IsHandled()
                    ? new BadRequestObjectResult(exception.ToError())
                    : new ObjectResult(exception.ToError()) { StatusCode = StatusCodes.Status500InternalServerError };
            }

            static Dictionary<string, IEnumerable<string>> Dictionary(IQueryCollection query)
            {
                return query.ToDictionary(kv => kv.Key, kv => kv.Value as IEnumerable<string>, StringComparer.OrdinalIgnoreCase);
            }
        }
    }
}
