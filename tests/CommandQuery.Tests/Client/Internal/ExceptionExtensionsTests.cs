using System.Net;
using System.Text;
using System.Text.Json;
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
            (await new HttpResponseMessage(HttpStatusCode.OK).EnsureSuccessAsync(CancellationToken.None)).Should().NotBeNull();

            var error = new Error("fail", new Dictionary<string, object> { { "foo", "bar" } });

            var subject = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent(JsonSerializer.Serialize(error), Encoding.UTF8, "application/json")
            };

            Func<Task> act = () => subject.EnsureSuccessAsync(CancellationToken.None);
            (await act.Should().ThrowAsync<CommandQueryException>())
                .WithMessage("StatusCode: 400, ReasonPhrase: 'Bad Request'*")
                .And.Error.Should().BeEquivalentTo(error);
        }
    }
}
