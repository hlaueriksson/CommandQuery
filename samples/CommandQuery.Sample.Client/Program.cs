using System;
using System.Threading.Tasks;
using CommandQuery.Client;
using CommandQuery.Sample.Contracts.Commands;
using CommandQuery.Sample.Contracts.Queries;
using Newtonsoft.Json;

namespace CommandQuery.Sample.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, press [Enter] to proceed...");
            Console.ReadLine();

            var commandClient = new CommandClient("http://localhost:7071/api/command/");

            commandClient.Post(new FooCommand { Value = "sv-SE" });
            await commandClient.PostAsync(new FooCommand { Value = "en-GB" });

            // Command with result
            commandClient.Post(new BazCommand { Value = "sv-SE" }).Log();
            (await commandClient.PostAsync(new BazCommand { Value = "en-GB" })).Log();

            var queryClient = new QueryClient("http://localhost:7071/api/query/", client =>
            {
                client.Timeout = TimeSpan.FromSeconds(10);
            });

            queryClient.Post(new BarQuery { Id = 1 }).Log();
            queryClient.Get(new BarQuery { Id = 1 }).Log();
            (await queryClient.PostAsync(new BarQuery { Id = 1 })).Log();
            (await queryClient.GetAsync(new BarQuery { Id = 1 })).Log();

            // Query with enumerable property
            queryClient.Get(new QuxQuery { Ids = new[] { Guid.NewGuid(), Guid.NewGuid() } }).Log();
            (await queryClient.GetAsync(new QuxQuery { Ids = new[] { Guid.NewGuid(), Guid.NewGuid() } })).Log();

            Console.WriteLine("Press [Enter] to exit");
            Console.ReadLine();
        }
    }

    public static class Extensions
    {
        public static void Log(this object result)
        {
            Console.WriteLine(JsonConvert.SerializeObject(result));
        }
    }
}