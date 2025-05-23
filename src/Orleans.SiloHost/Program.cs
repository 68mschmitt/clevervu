using Microsoft.Extensions.Configuration;
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

builder.UseOrleans((context, siloBuilder) =>
        {
            var config = context.Configuration;
            var orleansConnection = config.GetConnectionString("OrleansDB");
            siloBuilder.UseAdoNetClustering(options =>
            {
                options.Invariant = "System.Data.SqlClient";
                options.ConnectionString = orleansConnection;
            });

            siloBuilder.UseAdoNetReminderService(options =>
            {
                options.Invariant = "System.Data.SqlClient";
                options.ConnectionString = orleansConnection;
            });

            siloBuilder.AddAdoNetGrainStorage("GrainStorageForTest", options =>
            {
                options.Invariant = "System.Data.SqlClient";
                options.ConnectionString = orleansConnection;
            });

            siloBuilder.Configure<ClusterOptions>(options =>
            {
                options.ClusterId = "Clever-Cluster";
                options.ServiceId = "CleverVu";
            });

            siloBuilder
                .AddMemoryStreams("SMS")
                .AddMemoryGrainStorage("PubSubStore");
        });

await builder.RunConsoleAsync();
