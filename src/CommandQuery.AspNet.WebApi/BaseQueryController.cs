using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Tracing;
using CommandQuery.Internal;

namespace CommandQuery.AspNet.WebApi
{
    /// <summary>
    /// Base class for query controllers.
    /// </summary>
    public abstract class BaseQueryController : ApiController
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly ITraceWriter _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseQueryController" /> class.
        /// </summary>
        /// <param name="queryProcessor">An <see cref="IQueryProcessor" /></param>
        protected BaseQueryController(IQueryProcessor queryProcessor)
        {
            _queryProcessor = queryProcessor;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseQueryController" /> class.
        /// </summary>
        /// <param name="queryProcessor">An <see cref="IQueryProcessor" /></param>
        /// <param name="logger">An <see cref="ITraceWriter" /></param>
        protected BaseQueryController(IQueryProcessor queryProcessor, ITraceWriter logger)
        {
            _queryProcessor = queryProcessor;
            _logger = logger;
        }

        /// <summary>
        /// Handle a query.
        /// </summary>
        /// <param name="queryName">The name of the query</param>
        /// <param name="json">The JSON representation of the query</param>
        /// <returns>The result + 200, 400 or 500</returns>
        [HttpPost]
        [Route("{queryName}")]
        public async Task<IHttpActionResult> HandlePost(string queryName, [FromBody] Newtonsoft.Json.Linq.JObject json)
        {
            try
            {
                var result = await _queryProcessor.ProcessAsync<object>(queryName, json);

                return Ok(result);
            }
            catch (Exception exception)
            {
                _logger?.Error(Request, exception.GetQueryCategory(), exception, "Handle query failed: {QueryName}, {Payload}", queryName, json);

                return exception.IsHandled() ? (IHttpActionResult)BadRequest(exception.Message) : InternalServerError(exception);
            }
        }

        /// <summary>
        /// Handle a query.
        /// </summary>
        /// <param name="queryName">The name of the query</param>
        /// <returns>The result + 200, 400 or 500</returns>
        [HttpGet]
        [Route("{queryName}")]
        public async Task<IHttpActionResult> HandleGet(string queryName)
        {
            try
            {
                var result = await _queryProcessor.ProcessAsync<object>(queryName, Dictionary(Request.GetQueryNameValuePairs()));

                return Ok(result);
            }
            catch (Exception exception)
            {
                _logger?.Error(Request, exception.GetQueryCategory(), exception, "Handle query failed: {QueryName}, {Payload}", queryName, Request.GetQueryNameValuePairs());

                return exception.IsHandled() ? (IHttpActionResult)BadRequest(exception.Message) : InternalServerError(exception);
            }

            Dictionary<string, IEnumerable<string>> Dictionary(IEnumerable<KeyValuePair<string, string>> query)
            {
                return query
                    .GroupBy(kv => kv.Key, StringComparer.OrdinalIgnoreCase)
                    .ToDictionary(g => g.Key, g => g.Select(x => x.Value), StringComparer.OrdinalIgnoreCase);
            }
        }
    }
}