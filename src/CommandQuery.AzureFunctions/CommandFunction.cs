using System;
using System.Threading.Tasks;
using CommandQuery.Exceptions;

#if NET461
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
    public class CommandFunction
    {
        private readonly ICommandProcessor _commandProcessor;

        public CommandFunction(ICommandProcessor commandProcessor)
        {
            _commandProcessor = commandProcessor;
        }

        public async Task Handle(string commandName, string content)
        {
            await _commandProcessor.ProcessAsync(commandName, content);
        }

#if NET461
        public async Task<HttpResponseMessage> Handle(string commandName, HttpRequestMessage req, TraceWriter log)
        {
            log.Info($"Handle {commandName}");

            try
            {
                await Handle(commandName, await req.Content.ReadAsStringAsync());

                return req.CreateResponse(HttpStatusCode.OK);
            }
            catch (CommandProcessorException exception)
            {
                log.Error("Handle command failed", exception);

                return req.CreateErrorResponse(HttpStatusCode.BadRequest, exception.Message);
            }
            catch (CommandValidationException exception)
            {
                log.Error("Handle command failed", exception);

                return req.CreateErrorResponse(HttpStatusCode.BadRequest, exception.Message);
            }
            catch (Exception exception)
            {
                log.Error("Handle command failed", exception);

                return req.CreateErrorResponse(HttpStatusCode.InternalServerError, exception.Message);
            }
        }
#endif

#if NETSTANDARD2_0
        public async Task<IActionResult> Handle(string commandName, HttpRequest req, ILogger log)
        {
            log.LogInformation($"Handle {commandName}");

            try
            {
                await Handle(commandName, await req.ReadAsStringAsync());

                return new EmptyResult();
            }
            catch (CommandProcessorException exception)
            {
                log.LogError(exception, "Handle command failed");

                return new BadRequestObjectResult(exception.ToError());
            }
            catch (CommandValidationException exception)
            {
                log.LogError(exception, "Handle command failed");

                return new BadRequestObjectResult(exception.ToError());
            }
            catch (Exception exception)
            {
                log.LogError(exception, "Handle command failed");

                return new ObjectResult(exception.ToError())
                {
                    StatusCode = 500
                };
            }
        }
#endif
    }
}