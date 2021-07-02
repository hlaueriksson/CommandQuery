using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandQuery.Internal;

#if NET461
using System.Net;
using System.Net.Http;
using Microsoft.Azure.WebJobs.Host;
#endif

#if NETSTANDARD2_0 || NETCOREAPP3_0
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
#endif

namespace CommandQuery.AzureFunctions
{
    /// <summary>
    /// Handles queries for the Azure function.
    /// </summary>
    public interface IQueryFunction
    {
#if NET461
        /// <summary>
        /// Handle a query.
        /// </summary>
        /// <param name="queryName">The name of the query</param>
        /// <param name="req">A <see cref="HttpRequestMessage"/></param>
        /// <param name="log">A <see cref="TraceWriter"/></param>
        /// <returns>The result + 200, 400 or 500</returns>
        Task<HttpResponseMessage> Handle(string queryName, HttpRequestMessage req, TraceWriter log);
#endif

#if NETSTANDARD2_0 || NETCOREAPP3_0
        /// <summary>
        /// Handle a query.
        /// </summary>
        /// <param name="queryName">The name of the query</param>
        /// <param name="req">A <see cref="HttpRequest"/></param>
        /// <param name="log">An <see cref="ILogger"/></param>
        /// <returns>The result + 200, 400 or 500</returns>
        Task<IActionResult> Handle(string queryName, HttpRequest req, ILogger log);
#endif
    }

    /// <summary>
    /// Handles queries for the Azure function.
    /// </summary>
    public class QueryFunction : IQueryFunction
    {
        private readonly IQueryProcessor _queryProcessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryFunction"/> class.
        /// </summary>
        /// <param name="queryProcessor">An <see cref="IQueryProcessor"/></param>
        public QueryFunction(IQueryProcessor queryProcessor)
        {
            _queryProcessor = queryProcessor;
        }

#if NET461
        /// <summary>
        /// Handle a query.
        /// </summary>
        /// <param name="queryName">The name of the query</param>
        /// <param name="req">A <see cref="HttpRequestMessage"/></param>
        /// <param name="log">A <see cref="TraceWriter"/></param>
        /// <returns>The result + 200, 400 or 500</returns>
        public async Task<HttpResponseMessage> Handle(string queryName, HttpRequestMessage req, TraceWriter log)
        {
            log.Info($"Handle {queryName}");

            try
            {
                var result = req.Method == HttpMethod.Get
                    ? await Handle(queryName, Dictionary(req.GetQueryNameValuePairs()))
                    : await Handle(queryName, await req.Content.ReadAsStringAsync());

                return req.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception exception)
            {
                var payload = req.Method == HttpMethod.Get ? req.GetQueryNameValuePairs().ToJson() : await req.Content.ReadAsStringAsync();
                log.Error($"Handle query failed: {queryName}, {payload}", exception);

                return req.CreateResponse(exception.IsHandled() ? HttpStatusCode.BadRequest : HttpStatusCode.InternalServerError, exception.ToError());
            }

            Dictionary<string, IEnumerable<string>> Dictionary(IEnumerable<KeyValuePair<string, string>> query)
            {
                return query
                    .GroupBy(kv => kv.Key, StringComparer.OrdinalIgnoreCase)
                    .ToDictionary(g => g.Key, g => g.Select(x => x.Value), StringComparer.OrdinalIgnoreCase);
            }
        }
#endif

#if NETSTANDARD2_0 || NETCOREAPP3_0
        /// <summary>
        /// Handle a query.
        /// </summary>
        /// <param name="queryName">The name of the query</param>
        /// <param name="req">A <see cref="HttpRequest"/></param>
        /// <param name="log">An <see cref="ILogger"/></param>
        /// <returns>The result + 200, 400 or 500</returns>
        public async Task<IActionResult> Handle(string queryName, HttpRequest req, ILogger log)
        {
            log.LogInformation($"Handle {queryName}");

            try
            {
                var result = req.Method == "GET"
                    ? await Handle(queryName, Dictionary(req.Query))
                    : await Handle(queryName, await req.ReadAsStringAsync());

                return new OkObjectResult(result);
            }
            catch (Exception exception)
            {
                var payload = req.Method == "GET" ? (object)req.Query : await req.ReadAsStringAsync();
                log.LogError(exception.GetQueryEventId(), exception, "Handle query failed: {QueryName}, {Payload}", queryName, payload);

                return exception.IsHandled() ? new BadRequestObjectResult(exception.ToError()) : new ObjectResult(exception.ToError()) { StatusCode = 500 };
            }

            Dictionary<string, IEnumerable<string>> Dictionary(IQueryCollection query)
            {
                return query.ToDictionary(kv => kv.Key, kv => kv.Value as IEnumerable<string>, StringComparer.OrdinalIgnoreCase);
            }
        }
#endif

        private async Task<object> Handle(string queryName, string content)
        {
            return await _queryProcessor.ProcessAsync<object>(queryName, content);
        }

        private async Task<object> Handle(string queryName, IDictionary<string, IEnumerable<string>> query)
        {
            return await _queryProcessor.ProcessAsync<object>(queryName, query);
        }
    }
}