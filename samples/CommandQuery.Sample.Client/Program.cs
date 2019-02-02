using System;
using CommandQuery.Client;
using CommandQuery.Sample.Contracts.Commands;
using CommandQuery.Sample.Contracts.Queries;
using Newtonsoft.Json;

namespace CommandQuery.Sample.Client
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello, press enter to proceed...");
            Console.ReadLine();

            var commandClient = new CommandClient("https://commandquery-sample-azurefunctions-vs2.azurewebsites.net", x =>
                {
                    x.MaxResponseContentBufferSize = 1000000;
                });
            var queryClient = new QueryClient("https://commandquery-sample-azurefunctions-vs2.azurewebsites.net");

            //commandClient.Post(new FooCommand { Value = "sv-SE" });
            //var result = queryClient.Post(new BarQuery { Id = 1 });

            //Console.WriteLine(JsonConvert.SerializeObject(result));
            //Console.ReadLine();
        }
    }
}