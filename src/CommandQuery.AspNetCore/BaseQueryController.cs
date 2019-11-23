using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandQuery.AspNetCore.Internal;
using CommandQuery.Exceptions;
using CommandQuery.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;

namespace CommandQuery.AspNetCore
{
    /// <summary>
    /// Base class for query controllers.
    /// </summary>
    public abstract class BaseQueryController : Controller
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly ILogger<BaseQueryController> _logger;

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
        /// <param name="logger">An <see cref="ILogger" /></param>
        protected BaseQueryController(IQueryProcessor queryProcessor, ILogger<BaseQueryController> logger)
        {
            _queryProcessor = queryProcessor;
            _logger = logger;
        }

        /// <summary>
        /// Gets help.
        /// </summary>
        /// <returns>Query help</returns>
        [HttpGet]
        public IActionResult Help()
        {
            var baseUrl = Request.GetEncodedUrl();
            var queries = _queryProcessor.GetQueryTypes();

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
        public async Task<IActionResult> HandlePost(string queryName, [FromBody] Newtonsoft.Json.Linq.JObject json)
        {
            try
            {
                var result = await _queryProcessor.ProcessAsync<object>(queryName, json);

                return Ok(result);
            }
            catch (QueryProcessorException exception)
            {
                _logger?.LogError(LogEvents.QueryProcessorException, exception, "Handle query failed");

                return BadRequest(exception.ToError());
            }
            catch (QueryValidationException exception)
            {
                _logger?.LogError(LogEvents.QueryValidationException, exception, "Handle query failed");

                return BadRequest(exception.ToError());
            }
            catch (Exception exception)
            {
                _logger?.LogError(LogEvents.QueryException, exception, "Handle query failed");

                return StatusCode(500, exception.ToError()); // InternalServerError
            }
        }

        /// <summary>
        /// Handle a query.
        /// </summary>
        /// <param name="queryName">The name of the query</param>
        /// <returns>The result + 200, 400 or 500</returns>
        [HttpGet]
        [Route("{queryName}")]
        public async Task<IActionResult> HandleGet(string queryName)
        {
            try
            {
                var result = await _queryProcessor.ProcessAsync<object>(queryName, Dictionary(Request.Query));

                return Ok(result);
            }
            catch (QueryProcessorException exception)
            {
                _logger?.LogError(LogEvents.QueryProcessorException, exception, "Handle query failed");

                return BadRequest(exception.ToError());
            }
            catch (QueryValidationException exception)
            {
                _logger?.LogError(LogEvents.QueryValidationException, exception, "Handle query failed");

                return BadRequest(exception.ToError());
            }
            catch (Exception exception)
            {
                _logger?.LogError(LogEvents.QueryException, exception, "Handle query failed");

                return StatusCode(500, exception.ToError()); // InternalServerError
            }

            Dictionary<string, IEnumerable<string>> Dictionary(IQueryCollection query)
            {
                return query.ToDictionary(kv => kv.Key, kv => kv.Value as IEnumerable<string>, StringComparer.OrdinalIgnoreCase);
            }
        }
    }
}