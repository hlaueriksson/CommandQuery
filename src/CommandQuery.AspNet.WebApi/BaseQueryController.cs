using CommandQuery.Exceptions;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Tracing;
using CommandQuery.AspNet.WebApi.Internal;

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
        /// Gets help.
        /// </summary>
        /// <returns>Query help</returns>
        [HttpGet]
        [Route("")]
        public IHttpActionResult Help()
        {
            var baseUrl = Request.RequestUri.ToString();
            var queries = _queryProcessor.GetQueries();

            var result = queries.Select(x => new { query = x.Name, curl = x.GetCurl(baseUrl) });

            return Json(result);
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
            catch (QueryProcessorException exception)
            {
                _logger?.Error(Request, LogEvents.QueryProcessorException, exception, "Handle query failed");

                return BadRequest(exception.Message);
            }
            catch (QueryValidationException exception)
            {
                _logger?.Error(Request, LogEvents.QueryValidationException, exception, "Handle query failed");

                return BadRequest(exception.Message);
            }
            catch (Exception exception)
            {
                _logger?.Error(Request, LogEvents.QueryException, exception, "Handle query failed");

                return InternalServerError(exception);
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
                var result = await _queryProcessor.ProcessAsync<object>(queryName, Request.GetQueryNameValuePairs().ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase));

                return Ok(result);
            }
            catch (QueryProcessorException exception)
            {
                _logger?.Error(Request, LogEvents.QueryProcessorException, exception, "Handle query failed");

                return BadRequest(exception.Message);
            }
            catch (QueryValidationException exception)
            {
                _logger?.Error(Request, LogEvents.QueryValidationException, exception, "Handle query failed");

                return BadRequest(exception.Message);
            }
            catch (Exception exception)
            {
                _logger?.Error(Request, LogEvents.QueryException, exception, "Handle query failed");

                return InternalServerError(exception);
            }
        }
    }
}