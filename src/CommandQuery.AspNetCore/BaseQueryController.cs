using System;
using System.Linq;
using System.Threading.Tasks;
using CommandQuery.AspNetCore.Internal;
using CommandQuery.Exceptions;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CommandQuery.AspNetCore
{
    public abstract class BaseQueryController : Controller
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly ILogger<BaseQueryController> _logger;

        protected BaseQueryController(IQueryProcessor queryProcessor)
        {
            _queryProcessor = queryProcessor;
        }

        protected BaseQueryController(IQueryProcessor queryProcessor, ILogger<BaseQueryController> logger)
        {
            _queryProcessor = queryProcessor;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Help()
        {
            var baseUrl = Request.GetEncodedUrl();
            var queries = _queryProcessor.GetQueries();

            var result = queries.Select(x => new { query = x.Name, curl = x.GetCurl(baseUrl) });

            return Json(result);
        }

        [HttpPost]
        [Route("{queryName}")]
        public async Task<IActionResult> Handle(string queryName, [FromBody] Newtonsoft.Json.Linq.JObject json)
        {
            try
            {
                var result = await _queryProcessor.ProcessAsync<object>(queryName, json);

                return Ok(result);
            }
            catch (QueryProcessorException exception)
            {
                _logger?.LogError(LogEvents.QueryProcessorException, exception, "Handle query failed");

                return BadRequest(exception.Message);
            }
            catch (QueryValidationException exception)
            {
                _logger?.LogError(LogEvents.QueryValidationException, exception, "Handle query failed");

                return BadRequest(exception.Message);
            }
            catch (Exception exception)
            {
                _logger?.LogError(LogEvents.QueryException, exception, "Handle query failed");

                return StatusCode(500, exception.Message); // InternalServerError
            }
        }
    }
}