using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CommandQuery.Sample.Client
{
    public class LoggingHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Console.WriteLine(request.ToString());
            if (request.Content != null) Console.WriteLine(await request.Content.ReadAsStringAsync(cancellationToken));

            var response = await base.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode) Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine(response.ToString());
            Console.WriteLine(await response.Content.ReadAsStringAsync(cancellationToken));
            Console.ResetColor();
            Console.WriteLine();

            return response;
        }
    }
}
