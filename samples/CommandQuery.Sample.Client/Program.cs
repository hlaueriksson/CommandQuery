using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using CommandQuery.Client;
using CommandQuery.Sample.Contracts.Commands;
using CommandQuery.Sample.Contracts.Queries;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;

namespace CommandQuery.Sample.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, press [Enter] to proceed...");
            Console.ReadLine();

            //var commandClient = GetCommandClient();
            var commandClient = new CommandClient("http://localhost:7071/api/command/");

            await commandClient.PostAsync(new FooCommand { Value = "en-GB" });

            // Command with result
            (await commandClient.PostAsync(new BazCommand { Value = "en-GB" })).Log();

            var queryClient = new QueryClient("http://localhost:7071/api/query/", client =>
            {
                client.Timeout = TimeSpan.FromSeconds(10);
            });

            (await queryClient.PostAsync(new BarQuery { Id = 1 })).Log();
            (await queryClient.GetAsync(new BarQuery { Id = 1 })).Log();

            // Query with enumerable property
            (await queryClient.GetAsync(new QuxQuery { Ids = new[] { Guid.NewGuid(), Guid.NewGuid() } })).Log();

            // Query with nested property
            (await queryClient.GetAsync(new QuuxQuery { Id = 1, Corge = new Corge { DateTime = DateTime.UtcNow, Grault = new Grault { DayOfWeek = DayOfWeek.Monday } } })).Log();

            Console.WriteLine("Press [Enter] to exit");
            Console.ReadLine();
        }

        private static ICommandClient GetCommandClient()
        {
            var services = new ServiceCollection();
            services.AddHttpClient<ICommandClient, CommandClient>(client =>
            {
                client.BaseAddress = new Uri("http://localhost:7071/api/command/");
                client.Timeout = TimeSpan.FromSeconds(10);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            })
                .AddPolicyHandler(GetRetryPolicy())
                .AddPolicyHandler(GetCircuitBreakerPolicy());
            //services.AddSingleton(new JsonSerializerOptions(JsonSerializerDefaults.Web));

            return services.BuildServiceProvider().GetRequiredService<ICommandClient>();
        }

        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }

        static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
        }
    }

    public static class Extensions
    {
        public static void Log(this object result)
        {
            Console.WriteLine(JsonSerializer.Serialize(result));
        }
    }
}
