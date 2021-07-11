using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CommandQuery.Client;
using FluentAssertions;
using NUnit.Framework;

namespace CommandQuery.Tests.Client.Internal
{
    public class HttpResponseMessageExtensionsTests
    {
        [Test]
        public async Task EnsureSuccessStatusCode()
        {
            (await new HttpResponseMessage(HttpStatusCode.OK).EnsureSuccessAsync()).Should().NotBeNull();

            var error = new Error("fail", new Dictionary<string, object> { { "foo", "bar" } });

            var subject = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent(JsonSerializer.Serialize(error), Encoding.UTF8, "application/json")
            };
            subject.Invoking(x => x.EnsureSuccessAsync())
                .Should().Throw<CommandQueryException>()
                .WithMessage("StatusCode: 400, ReasonPhrase: 'Bad Request'*")
                .And.Error.Should().BeEquivalentTo(error);
        }
    }
}
