using System;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using CommandQuery.AzureFunctions.Internal;
using CommandQuery.Exceptions;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json.Linq;

#if NET46
using System.Net;
using System.Net.Http;
#endif

#if NETSTANDARD2_0
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
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
            await _commandProcessor.ProcessAsync(commandName, JObject.Parse(content));
        }

#if NET46
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
        public async Task<IActionResult> Handle(string commandName, HttpRequest req, TraceWriter log)
        {
            log.Info($"Handle {commandName}");

            try
            {
                await Handle(commandName, await req.ReadAsStringAsync());

                return new EmptyResult();
            }
            catch (CommandProcessorException exception)
            {
                log.Error("Handle command failed", exception);

                return new BadRequestObjectResult(exception.Message);
            }
            catch (CommandValidationException exception)
            {
                log.Error("Handle command failed", exception);

                return new BadRequestObjectResult(exception.Message);
            }
            catch (Exception exception)
            {
                log.Error("Handle command failed", exception);

                return new ObjectResult(exception.Message)
                {
                    StatusCode = 500
                };
            }
        }
#endif
    }

    public class CommandServiceProvider : IServiceProvider
    {
        private IContainer _container;

        public CommandServiceProvider(IContainer container)
        {
            _container = container;
        }

        public object GetService(Type serviceType)
        {
            return _container.Resolve(serviceType);
        }
    }

    public static class CommandExtensions
    {
        public static ICommandProcessor GetCommandProcessor(this Assembly assembly)
        {
            var builder = new ContainerBuilder();
            builder.AddCommands(assembly);

            return new CommandProcessor(new CommandTypeCollection(assembly), new CommandServiceProvider(builder.Build()));
        }

        public static ICommandProcessor GetCommandProcessor(this Assembly[] assemblies)
        {
            var builder = new ContainerBuilder();
            builder.AddCommands(assemblies);

            return new CommandProcessor(new CommandTypeCollection(assemblies), new CommandServiceProvider(builder.Build()));
        }
    }
}