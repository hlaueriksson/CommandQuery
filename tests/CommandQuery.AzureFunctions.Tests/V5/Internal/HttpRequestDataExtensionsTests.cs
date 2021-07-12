#if NET5_0
using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Core.Serialization;
using CommandQuery.Exceptions;
using FluentAssertions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace CommandQuery.AzureFunctions.Tests.V5.Internal
{
    public class HttpRequestDataExtensionsTests
    {
        private HttpRequestData Req;

        [SetUp]
        public void SetUp()
        {
            var options = new Mock<IOptions<WorkerOptions>>();
            options.Setup(x => x.Value).Returns(new WorkerOptions { Serializer = new JsonObjectSerializer() });
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(x => x.GetService(typeof(IOptions<WorkerOptions>))).Returns(options.Object);
            var context = new Mock<FunctionContext>();
            context.Setup(x => x.InstanceServices).Returns(serviceProvider.Object);

            Req = new FakeHttpRequestData(context.Object);
        }

        [Test]
        public async Task OkAsync()
        {
            var response = await Req.OkAsync(new { Foo = "Bar" });
            response.StatusCode.Should().Be(200);
            response.Body.Position = 0;
            var result = await new StreamReader(response.Body).ReadToEndAsync();
            result.Should().Be("{\"Foo\":\"Bar\"}");
        }

        [Test]
        public async Task BadRequestAsync()
        {
            var exception = new CustomCommandException("fail") { Foo = "Bar" };
            var response = await Req.BadRequestAsync(exception);
            response.StatusCode.Should().Be(400);
            response.Body.Position = 0;
            var result = await new StreamReader(response.Body).ReadToEndAsync();
            result.Should().Be("{\"Message\":\"fail\",\"Details\":{\"Foo\":\"Bar\"}}");
        }

        [Test]
        public async Task InternalServerErrorAsync()
        {
            var exception = new CustomCommandException("fail") { Foo = "Bar" };
            var response = await Req.InternalServerErrorAsync(exception);
            response.StatusCode.Should().Be(500);
            response.Body.Position = 0;
            var result = await new StreamReader(response.Body).ReadToEndAsync();
            result.Should().Be("{\"Message\":\"fail\",\"Details\":{\"Foo\":\"Bar\"}}");
        }
    }

    public class CustomCommandException : CommandException
    {
        public string Foo { get; set; }

        public CustomCommandException(string message) : base(message)
        {
        }
    }
}
#endif
