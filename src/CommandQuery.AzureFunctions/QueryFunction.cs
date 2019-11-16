using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommandQuery.Exceptions;

#if NET461
using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.Azure.WebJobs.Host;
#endif

#if NETSTANDARD2_0
using CommandQuery.Internal;
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
    public class QueryFunction
    {
        private readonly IQueryProcessor _queryProcessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryFunction" /> class.
        /// </summary>
        /// <param name="queryProcessor">An <see cref="IQueryProcessor" /></param>
        public QueryFunction(IQueryProcessor queryProcessor)
        {
            _queryProcessor = queryProcessor;
        }

#if NET461
        /// <summary>
        /// Handle a query.
        /// </summary>
        /// <param name="queryName">The name of the query</param>
        /// <param name="req">A <see cref="HttpRequestMessage" /></param>
        /// <param name="log">A <see cref="TraceWriter" /></param>
        /// <returns>The result + 200, 400 or 500</returns>
        public async Task<HttpResponseMessage> Handle(string queryName, HttpRequestMessage req, TraceWriter log)
        {
            log.Info($"Handle {queryName}");

            try
            {
                var result = req.Method == HttpMethod.Get
                    ? await Handle(queryName, req.GetQueryNameValuePairs().ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase))
                    : await Handle(queryName, await req.Content.ReadAsStringAsync());

                return req.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (QueryProcessorException exception)
            {
                log.Error("Handle query failed", exception);

                return req.CreateErrorResponse(HttpStatusCode.BadRequest, exception.Message);
            }
            catch (QueryValidationException exception)
            {
                log.Error("Handle query failed", exception);

                return req.CreateErrorResponse(HttpStatusCode.BadRequest, exception.Message);
            }
            catch (Exception exception)
            {
                log.Error("Handle query failed", exception);

                return req.CreateErrorResponse(HttpStatusCode.InternalServerError, exception.Message);
            }
        }
#endif

#if NETSTANDARD2_0
        /// <summary>
        /// Handle a query.
        /// </summary>
        /// <param name="queryName">The name of the query</param>
        /// <param name="req">A <see cref="HttpRequest" /></param>
        /// <param name="log">An <see cref="ILogger" /></param>
        /// <returns>The result + 200, 400 or 500</returns>
        public async Task<IActionResult> Handle(string queryName, HttpRequest req, ILogger log)
        {
            log.LogInformation($"Handle {queryName}");

            try
            {
                var result = req.Method == "GET"
                    ? await Handle(queryName, req.GetQueryParameterDictionary())
                    : await Handle(queryName, await req.ReadAsStringAsync());

                return new OkObjectResult(result);
            }
            catch (QueryProcessorException exception)
            {
                log.LogError(exception, "Handle query failed");

                return new BadRequestObjectResult(exception.ToError());
            }
            catch (QueryValidationException exception)
            {
                log.LogError(exception, "Handle query failed");

                return new BadRequestObjectResult(exception.ToError());
            }
            catch (Exception exception)
            {
                log.LogError(exception, "Handle query failed");

                return new ObjectResult(exception.ToError())
                {
                    StatusCode = 500
                };
            }
        }
#endif

        private async Task<object> Handle(string queryName, string content)
        {
            return await _queryProcessor.ProcessAsync<object>(queryName, content);
        }

        private async Task<object> Handle(string queryName, IDictionary<string, string> query)
        {
            return await _queryProcessor.ProcessAsync<object>(queryName, query);
        }
    }
}