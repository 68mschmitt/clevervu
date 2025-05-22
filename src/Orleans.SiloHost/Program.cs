using Microsoft.Extensions.Hosting;
using Orleans.Configuration;

var invariant = "System.Data.SqlClient";
var connectionString = "server=localhost,1433; database=ORLEANS; user id=sa; password=changeMe!; TrustServerCertificate=true;";

await Host.CreateDefaultBuilder(args)
    .UseOrleans(siloBuilder =>
    {
        siloBuilder.UseAdoNetClustering(options =>
        {
            options.Invariant = invariant;
            options.ConnectionString = connectionString;
        });

        siloBuilder.UseAdoNetReminderService(options =>
        {
            options.Invariant = invariant;
            options.ConnectionString = connectionString;
        });

        siloBuilder.AddAdoNetGrainStorage("GrainStorageForTest", options =>
        {
            options.Invariant = invariant;
            options.ConnectionString = connectionString;
        });

        siloBuilder.Configure<ClusterOptions>(options =>
        {
            options.ClusterId = "my-first-cluster";
            options.ServiceId = "SampleApp";
        });
    })
    .RunConsoleAsync();
