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

            var commandClient = new CommandClient("https://commandquery-sample-azurefunctions-vs2.azurewebsites.net/api/command/", x =>
                {
                    x.MaxResponseContentBufferSize = 1000000;
                });
            var queryClient = new QueryClient("https://commandquery-sample-azurefunctions-vs2.azurewebsites.net/api/query/");

            commandClient.Post(new FooCommand { Value = "sv-SE" });
            queryClient.Post(new BarQuery { Id = 1 }).Log();
            queryClient.Get(new BarQuery { Id = 1 }).Log();

            await commandClient.PostAsync(new FooCommand { Value = "en-GB" });
            (await queryClient.PostAsync(new BarQuery { Id = 1 })).Log();
            (await queryClient.GetAsync(new BarQuery { Id = 1 })).Log();

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