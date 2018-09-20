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
    public abstract class BaseQueryController : ApiController
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly ITraceWriter _logger;

        protected BaseQueryController(IQueryProcessor queryProcessor)
        {
            _queryProcessor = queryProcessor;
        }

        protected BaseQueryController(IQueryProcessor queryProcessor, ITraceWriter logger)
        {
            _queryProcessor = queryProcessor;
            _logger = logger;
        }

        [HttpGet]
        [Route("")]
        public IHttpActionResult Help()
        {
            var baseUrl = Request.RequestUri.ToString();
            var queries = _queryProcessor.GetQueries();

            var result = queries.Select(x => new { query = x.Name, curl = x.GetCurl(baseUrl) });

            return Json(result);
        }

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