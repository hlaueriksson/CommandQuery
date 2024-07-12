using System.Text;
using System.Text.Json;
using CommandQuery.AzureFunctions;
using CommandQuery.Sample.Contracts.Commands;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace CommandQuery.Sample.AzureFunctions.Tests
{
    public class CommandTests
    {
        [SetUp]
        public void SetUp()
        {
            var serviceCollection = new ServiceCollection();
            Program.ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            Subject = new Command(serviceProvider.GetRequiredService<ICommandFunction>());

            var context = new Mock<FunctionContext>();
            context.SetupProperty(c => c.InstanceServices, serviceProvider);
            Context = context.Object;
        }

        [Test]
        public async Task should_handle_command()
        {
            var result = await Subject.Run(GetRequest(new FooCommand { Value = "Foo" }), Context, "FooCommand");
            result.As<IStatusCodeActionResult>().StatusCode.Should().Be(200);
        }

        [Test]
        public async Task should_handle_errors()
        {
            var result = await Subject.Run(GetRequest(new FooCommand()), Context, "FooCommand");
            result.ShouldBeError("Value cannot be null or empty");
        }

        HttpRequest GetRequest(object body)
        {
            var request = new Mock<HttpRequest>();
            request.Setup(r => r.Body).Returns(new MemoryStream(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(body))));
            return request.Object;
        }

        Command Subject = null!;
        FunctionContext Context = null!;
    }
}
