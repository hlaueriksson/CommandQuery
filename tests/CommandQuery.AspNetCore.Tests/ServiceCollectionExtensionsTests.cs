using System;
using System.Reflection;
using CommandQuery.Tests;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace CommandQuery.AspNetCore.Tests
{
    public class ServiceCollectionExtensionsTests
    {
        [Test]
        public void when_AddCommandControllers()
        {
            var assembly = typeof(FakeCommandHandler).Assembly;
            var serviceCollection = new ServiceCollection();

            serviceCollection
                .AddCommandControllers(assembly)
                .AddLogging(logging => logging.AddConsole());
            var provider = serviceCollection.BuildServiceProvider();
            var activator = provider.GetService<IControllerActivator>();

            CreateController(activator, provider, typeof(CommandController<FakeCommand>)).Should().NotBeNull();
            CreateController(activator, provider, typeof(CommandController<FakeResultCommand, FakeResult>)).Should().NotBeNull();
        }

        [Test]
        public void when_AddQueryControllers()
        {
            var assembly = typeof(FakeQueryHandler).Assembly;
            var serviceCollection = new ServiceCollection();

            serviceCollection
                .AddQueryControllers(assembly)
                .AddLogging(logging => logging.AddConsole());
            var provider = serviceCollection.BuildServiceProvider();
            var activator = provider.GetService<IControllerActivator>();

            CreateController(activator, provider, typeof(QueryController<FakeQuery, FakeResult>)).Should().NotBeNull();
        }

        private static object CreateController(IControllerActivator activator, ServiceProvider provider, Type controllerType)
        {
            var actionContext = new ActionContext(
                new DefaultHttpContext
                {
                    RequestServices = provider
                },
                new RouteData(),
                new ControllerActionDescriptor
                {
                    ControllerTypeInfo = controllerType.GetTypeInfo()
                });
            return activator.Create(new ControllerContext(actionContext));
        }
    }
}
