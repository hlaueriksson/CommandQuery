using CommandQuery;
using CommandQuery.AspNetCore;
using CommandQuery.Sample.Contracts.Commands;
using CommandQuery.Sample.Contracts.Queries;
using CommandQuery.Sample.Handlers;
using CommandQuery.Sample.Handlers.Commands;
using CommandQuery.Sample.Handlers.Queries;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add commands and queries
builder.Services.AddCommandControllers(typeof(FooCommandHandler).Assembly, typeof(FooCommand).Assembly);
builder.Services.AddQueryControllers(typeof(BarQueryHandler).Assembly, typeof(BarQuery).Assembly);

// Add handler dependencies
builder.Services.AddTransient<IDateTimeProxy, DateTimeProxy>();
builder.Services.AddTransient<ICultureService, CultureService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Validation
app.Services.GetService<ICommandProcessor>()!.AssertConfigurationIsValid();
app.Services.GetService<IQueryProcessor>()!.AssertConfigurationIsValid();

app.Run();
