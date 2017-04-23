using System;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using CommandQuery.AzureFunctions.Internal;
using CommandQuery.Exceptions;
using Newtonsoft.Json.Linq;

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
        public async Task<HttpResponseMessage> Handle(string commandName, HttpRequestMessage req, Microsoft.Azure.WebJobs.Host.TraceWriter log)
        {
            log.Info($"Handle {commandName}");

            try
            {
                await Handle(commandName, await req.Content.ReadAsStringAsync());

                return req.CreateResponse(HttpStatusCode.OK, string.Empty);
            }
            catch (CommandProcessorException exception)
            {
                log.Error("Handle command failed", exception);

                return req.CreateResponse(HttpStatusCode.BadRequest, exception.Message);
            }
            catch (CommandValidationException exception)
            {
                log.Error("Handle command failed", exception);

                return req.CreateResponse(HttpStatusCode.BadRequest, "Validation error: " + exception.Message);
            }
            catch (Exception exception)
            {
                log.Error("Handle command failed", exception);

                return req.CreateResponse(HttpStatusCode.InternalServerError, "Error: " + exception.Message);
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