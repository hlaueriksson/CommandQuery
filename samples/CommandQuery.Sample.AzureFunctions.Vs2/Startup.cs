using CommandQuery.AzureFunctions;
using CommandQuery.DependencyInjection;
using CommandQuery.Sample.AzureFunctions.Vs2;
using CommandQuery.Sample.Contracts.Commands;
using CommandQuery.Sample.Contracts.Queries;
using CommandQuery.Sample.Handlers;
using CommandQuery.Sample.Handlers.Commands;
using CommandQuery.Sample.Handlers.Queries;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]

namespace CommandQuery.Sample.AzureFunctions.Vs2
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<ICommandFunction, CommandFunction>();
            builder.Services.AddSingleton<IQueryFunction, QueryFunction>();

            // Add commands and queries
            builder.Services.AddCommands(typeof(FooCommandHandler).Assembly, typeof(FooCommand).Assembly);
            builder.Services.AddQueries(typeof(BarQueryHandler).Assembly, typeof(BarQuery).Assembly);

            // Add handler dependencies
            builder.Services.AddTransient<IDateTimeProxy, DateTimeProxy>();
            builder.Services.AddTransient<ICultureService, CultureService>();
        }
    }
}