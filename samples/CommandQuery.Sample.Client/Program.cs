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
        private static IServiceProvider _serviceProvider = null!;

        public static async Task Main(string[] args)
        {
            ConfigureServices(args.Any() ? args : new[] { "http://localhost:7071/api" });

            var commandClient = _serviceProvider.GetRequiredService<ICommandClient>();
            var queryClient = _serviceProvider.GetRequiredService<IQueryClient>();

            await commandClient.PostAsync(new FooCommand { Value = "en-GB" }); // Command without result
            await commandClient.PostAsync(new BazCommand { Value = "en-GB" }); // Command with result

            await queryClient.PostAsync(new BarQuery { Id = 1 }); // Query via POST
            await queryClient.GetAsync(new BarQuery { Id = 1 }); // Query via GET
            await queryClient.GetAsync(new QuxQuery { Ids = new[] { Guid.NewGuid(), Guid.NewGuid() } }); // Query with enumerable property
            await queryClient.GetAsync(new QuuxQuery { Id = 1, Corge = new Corge { DateTime = DateTime.UtcNow, Grault = new Grault { DayOfWeek = DayOfWeek.Monday } } }); // Query with nested property
        }

        static void ConfigureServices(params string[] baseUrls)
        {
            var commandUrl = baseUrls.Length == 1 ? $"{baseUrls.Single()}/command/" : baseUrls.First();
            var queryUrl = baseUrls.Length == 1 ? $"{baseUrls.Single()}/query/" : baseUrls.Last();

            var services = new ServiceCollection();
            services.AddTransient<LoggingHandler>();
            //services.AddSingleton(new JsonSerializerOptions(JsonSerializerDefaults.Web));
            services.AddHttpClient<ICommandClient, CommandClient>(x =>
                {
                    x.BaseAddress = new Uri(commandUrl);
                    x.Timeout = TimeSpan.FromSeconds(10);
                })
                .AddHttpMessageHandler<LoggingHandler>()
                .AddPolicyHandler(GetRetryPolicy());
            services.AddHttpClient<IQueryClient, QueryClient>(x =>
                {
                    x.BaseAddress = new Uri(queryUrl);
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
