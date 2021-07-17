using System;
using System.Linq;
using System.Net.Http;
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
        private static IServiceProvider _serviceProvider;

        public static async Task Main(string[] args)
        {
            var baseUrl = args.Any() ? args.First() : "http://localhost:7071/api";

            Console.WriteLine($"CommandQuery.Sample.Client: {baseUrl}");

            ConfigureServices(baseUrl);

            var commandClient = _serviceProvider.GetRequiredService<ICommandClient>();
            var queryClient = _serviceProvider.GetRequiredService<IQueryClient>();

            await commandClient.PostAsync(new FooCommand { Value = "en-GB" }); // Command without result
            await commandClient.PostAsync(new BazCommand { Value = "en-GB" }); // Command with result

            await queryClient.PostAsync(new BarQuery { Id = 1 }); // Query via POST
            await queryClient.GetAsync(new BarQuery { Id = 1 }); // Query via GET
            await queryClient.GetAsync(new QuxQuery { Ids = new[] { Guid.NewGuid(), Guid.NewGuid() } }); // Query with enumerable property
            await queryClient.GetAsync(new QuuxQuery { Id = 1, Corge = new Corge { DateTime = DateTime.UtcNow, Grault = new Grault { DayOfWeek = DayOfWeek.Monday } } }); // Query with nested property
        }

        static void ConfigureServices(string baseUrl)
        {
            var services = new ServiceCollection();
            services.AddTransient<LoggingHandler>();
            //services.AddSingleton(new JsonSerializerOptions(JsonSerializerDefaults.Web));
            services.AddHttpClient<ICommandClient, CommandClient>(x =>
                {
                    x.BaseAddress = new Uri($"{baseUrl}/command/");
                    x.Timeout = TimeSpan.FromSeconds(10);
                })
                .AddHttpMessageHandler<LoggingHandler>()
                .AddPolicyHandler(GetRetryPolicy());
            services.AddHttpClient<IQueryClient, QueryClient>(x =>
                {
                    x.BaseAddress = new Uri($"{baseUrl}/query/");
                    x.Timeout = TimeSpan.FromSeconds(10);
                })
                .AddHttpMessageHandler<LoggingHandler>()
                .AddPolicyHandler(GetRetryPolicy());

            _serviceProvider = services.BuildServiceProvider();
        }

        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }
    }
}
