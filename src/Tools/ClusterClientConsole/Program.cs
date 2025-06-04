using ClusterClientConsole;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans.Configuration;

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

// Running Client Code

IGrainFactory client = host.Services.GetRequiredService<IGrainFactory>();

Console.WriteLine("Welcome to the CleverVu Orleans test project REPL");
string? line;
string[] commands =
[
    "greeting",
    "update",
    "exit"
];

while (true)
{
    Console.WriteLine("Commands:");
    foreach(var command in commands) Console.WriteLine($"  {command}");

    Console.Write("> ");
    line = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(line))
        continue;

    var tokens = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    if (tokens.Length == 0) continue;

    switch (tokens[0].ToLower())
    {
        case "exit":
        case "quit":
            Console.WriteLine("Exiting...");
            host.Dispose();
            return;
        case "greeting":
            await GreetingGrainRepl.StartReplAsync(client);
            break;
        case "update":
            await UpdateRepl.StartReplAsync(client);
            break;
        default:
            Console.WriteLine("Unknown command.");
            break;
    }
}
