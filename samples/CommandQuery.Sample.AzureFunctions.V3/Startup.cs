using CommandQuery.AzureFunctions;
using CommandQuery.Sample.AzureFunctions.V3;
using CommandQuery.Sample.Contracts.Commands;
using CommandQuery.Sample.Contracts.Queries;
using CommandQuery.Sample.Handlers;
using CommandQuery.Sample.Handlers.Commands;
using CommandQuery.Sample.Handlers.Queries;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

[assembly: FunctionsStartup(typeof(Startup))]

namespace CommandQuery.Sample.AzureFunctions.V3
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            //builder.Services.AddSingleton(new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });

            // Add commands and queries
            builder.Services.AddCommandFunction(typeof(FooCommandHandler).Assembly, typeof(FooCommand).Assembly);
            builder.Services.AddQueryFunction(typeof(BarQueryHandler).Assembly, typeof(BarQuery).Assembly);

            // Add handler dependencies
            builder.Services.AddTransient<IDateTimeProxy, DateTimeProxy>();
            builder.Services.AddTransient<ICultureService, CultureService>();
        }
    }
}
