using System.Text;
using FluentAssertions;
using LoFuUnit.NUnit;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;

namespace CommandQuery.AzureFunctions.Tests.Internal
{
    public class HttpRequestExtensionsTests
    {
        [LoFu, Test]
        public async Task ReadAsStringAsync()
        {
            async Task should_return_a_string()
            {
                var mock = new Mock<HttpRequest>();
                mock.SetupGet(x => x.Body).Returns(new MemoryStream(Encoding.UTF8.GetBytes("{}")));
                var req = mock.Object;

                var result = await req.ReadAsStringAsync();
                result.Should().Be("{}");
            }

            async Task should_throw_when_request_is_null()
            {
                Func<Task> act = () => ((HttpRequest)null).ReadAsStringAsync();
                await act.Should().ThrowAsync<ArgumentNullException>();
            }

            async Task should_return_null_when_body_is_null()
            {
                var result = await new Mock<HttpRequest>().Object.ReadAsStringAsync();
                result.Should().BeNull();
            }
        }
    }
}
