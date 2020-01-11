﻿using System;
using System.Threading.Tasks;
using CommandQuery.AspNetCore.Internal;
using CommandQuery.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CommandQuery.AspNetCore
{
    /// <summary>
    /// Base class for query controllers.
    /// </summary>
    [ApiController]
    [Route("api/query/[controller]")]
    internal class QueryController<TQuery, TResult> : ControllerBase where TQuery : IQuery<TResult>
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly ILogger<QueryController<TQuery, TResult>> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryController&lt;TQuery, TResult&gt;" /> class.
        /// </summary>
        /// <param name="queryProcessor">An <see cref="IQueryProcessor" /></param>
        /// <param name="logger">An <see cref="ILogger" /></param>
        public QueryController(IQueryProcessor queryProcessor, ILogger<QueryController<TQuery, TResult>> logger)
        {
            _queryProcessor = queryProcessor;
            _logger = logger;
        }

        /// <summary>
        /// Handle a query.
        /// </summary>
        /// <param name="query">The query</param>
        /// <returns>The result + 200, 400 or 500</returns>
        [HttpPost]
        public async Task<IActionResult> HandlePost(TQuery query)
        {
            try
            {
                var result = await _queryProcessor.ProcessAsync(query);

                return Ok(result);
            }
            catch (QueryProcessorException exception)
            {
                _logger?.LogError(LogEvents.QueryProcessorException, exception, "Handle query failed");

                return BadRequest(exception.ToError());
            }
            catch (QueryException exception)
            {
                _logger?.LogError(LogEvents.QueryException, exception, "Handle query failed");

                return BadRequest(exception.ToError());
            }
            catch (Exception exception)
            {
                _logger?.LogError(LogEvents.UnhandledQueryException, exception, "Handle query failed");

                return StatusCode(500, exception.ToError()); // InternalServerError
            }
        }

        /// <summary>
        /// Handle a query.
        /// </summary>
        /// <param name="query">The query</param>
        /// <returns>The result + 200, 400 or 500</returns>
        [HttpGet]
        public async Task<IActionResult> HandleGet([FromQuery] TQuery query)
        {
            try
            {
                var result = await _queryProcessor.ProcessAsync(query);

                return Ok(result);
            }
            catch (QueryProcessorException exception)
            {
                _logger?.LogError(LogEvents.QueryProcessorException, exception, "Handle query failed");

                return BadRequest(exception.ToError());
            }
            catch (QueryException exception)
            {
                _logger?.LogError(LogEvents.QueryException, exception, "Handle query failed");

                return BadRequest(exception.ToError());
            }
            catch (Exception exception)
            {
                _logger?.LogError(LogEvents.UnhandledQueryException, exception, "Handle query failed");

                return StatusCode(500, exception.ToError()); // InternalServerError
            }
        }
    }
}