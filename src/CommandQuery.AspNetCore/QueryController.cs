using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CommandQuery.AspNetCore
{
    /// <summary>
    /// Base class for query controllers.
    /// </summary>
    [ApiController]
    [Route("api/query/[controller]")]
    internal class QueryController<TQuery, TResult> : ControllerBase
        where TQuery : IQuery<TResult>
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryController{TQuery,TResult}"/> class.
        /// </summary>
        /// <param name="queryProcessor">An <see cref="IQueryProcessor"/>.</param>
        /// <param name="logger">An <see cref="ILogger"/>.</param>
        public QueryController(IQueryProcessor queryProcessor, ILogger<QueryController<TQuery, TResult>> logger)
        {
            _queryProcessor = queryProcessor;
            _logger = logger;
        }

        /// <summary>
        /// Handle a query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The result + 200, 400 or 500.</returns>
        [HttpPost]
        public async Task<IActionResult> HandlePostAsync(TQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handle {@Query}", query);

            try
            {
                var result = await _queryProcessor.ProcessAsync(query, cancellationToken).ConfigureAwait(false);

                return Ok(result);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Handle query failed: {@Query}", query);

                return exception.IsHandled() ? BadRequest(exception.ToError()) : StatusCode(500, exception.ToError());
            }
        }

        /// <summary>
        /// Handle a query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The result + 200, 400 or 500.</returns>
        [HttpGet]
        public async Task<IActionResult> HandleGetAsync([FromQuery] TQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handle {@Query}", query);

            try
            {
                var result = await _queryProcessor.ProcessAsync(query, cancellationToken).ConfigureAwait(false);

                return Ok(result);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Handle query failed: {@Query}", query);

                return exception.IsHandled() ? BadRequest(exception.ToError()) : StatusCode(500, exception.ToError());
            }
        }
    }
}
