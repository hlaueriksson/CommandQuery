using CommandQuery.Client;
using FluentAssertions;

namespace CommandQuery.Tests.Client.Internal
{
    public class HttpClientExtensionsTests
    {
        [Test]
        public void SetDefaultRequestHeaders()
        {
            var client = new HttpClient();
            client.SetDefaultRequestHeaders();
            client.DefaultRequestHeaders.Accept.Single().MediaType.Should().Be("application/json");
            client.DefaultRequestHeaders.UserAgent.Single().Product.Name.Should().StartWith("CommandQuery.Client");

            Action act = () => ((HttpClient)null).SetDefaultRequestHeaders();
            act.Should().Throw<ArgumentNullException>();
        }
    }
}
