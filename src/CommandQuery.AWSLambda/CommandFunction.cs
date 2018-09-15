using System;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using CommandQuery.AWSLambda.Internal;
using CommandQuery.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace CommandQuery.AWSLambda
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

        public async Task<APIGatewayProxyResponse> Handle(string commandName, APIGatewayProxyRequest request, ILambdaContext context)
        {
            context.Logger.LogLine($"Handle {commandName}");

            try
            {
                await Handle(commandName, request.Body);

                return new APIGatewayProxyResponse { StatusCode = (int)HttpStatusCode.OK };
            }
            catch (CommandProcessorException exception)
            {
                context.Logger.LogLine("Handle command failed: " + exception);

                return exception.ToBadRequest();
            }
            catch (CommandValidationException exception)
            {
                context.Logger.LogLine("Handle command failed: " + exception);

                return exception.ToBadRequest();
            }
            catch (Exception exception)
            {
                context.Logger.LogLine("Handle command failed: " + exception);

                return exception.ToInternalServerError();
            }
        }
    }

    public static class CommandExtensions
    {
        public static ICommandProcessor GetCommandProcessor(this Assembly assembly)
        {
            return GetCommandProcessor(new ServiceCollection(), assembly);
        }

        public static ICommandProcessor GetCommandProcessor(this Assembly[] assemblies)
        {
            return GetCommandProcessor(new ServiceCollection(), assemblies);
        }

        public static ICommandProcessor GetCommandProcessor(this Assembly assembly, IServiceCollection services)
        {
            return GetCommandProcessor(services, assembly);
        }

        public static ICommandProcessor GetCommandProcessor(this Assembly[] assemblies, IServiceCollection services)
        {
            return GetCommandProcessor(services, assemblies);
        }

        private static ICommandProcessor GetCommandProcessor(IServiceCollection services, params Assembly[] assemblies)
        {
            services.AddCommands(assemblies);

            return new CommandProcessor(new CommandTypeCollection(assemblies), services.BuildServiceProvider());
        }
    }
}