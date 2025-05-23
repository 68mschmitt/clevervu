using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans.Configuration;
using OrleansGrainInterfaces.Example;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureAppConfiguration((hostingContext, config) =>
{
    var env = hostingContext.HostingEnvironment;
    config
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings{env.EnvironmentName}.json", optional: true)
        .AddEnvironmentVariables();

    if (env.IsDevelopment())
        config.AddUserSecrets<Program>();
});

builder.UseOrleansClient((context, clientBuilder) =>
{
    var config = context.Configuration;
    var orleansConnection = config.GetConnectionString("OrleansDB");

    clientBuilder.Configure<ClusterOptions>(options =>
    {
        options.ClusterId = "Clever-Cluster";
        options.ServiceId = "CleverVu";
    });

    clientBuilder.UseAdoNetClustering(options =>
    {
        options.Invariant = "System.Data.SqlClient";
        options.ConnectionString = orleansConnection;
    });

    clientBuilder.AddMemoryStreams("SMS");
});

var host = builder.UseConsoleLifetime().Build();

await host.StartAsync();

IGrainFactory client = host.Services.GetRequiredService<IGrainFactory>();

List<IGreetingGrain> greetingGrains = [];

Console.WriteLine("How many grains do you want to cache?");
var killCount = int.Parse(Console.ReadLine( )?? "1000");

for (int i = 0; i < killCount; i++)
    greetingGrains.Add(client.GetGrain<IGreetingGrain>($"Mike: {i}"));

var getTasks = Task.FromResult(greetingGrains.Select(g => g.GetAndWait()));

await Task.WhenAll(getTasks);

foreach (var greetingGrain in greetingGrains)
{
    Console.WriteLine(await greetingGrain.GetAsync());
}

Console.WriteLine("Do you want to kill the grains?");
var choice = Console.ReadLine();

if (choice == "y")
{
    foreach (var greetingGrain in greetingGrains)
    {
        _ = Task.Run(greetingGrain.DeactivateAsync);
    }
}

Console.ReadLine();

host.Dispose();
