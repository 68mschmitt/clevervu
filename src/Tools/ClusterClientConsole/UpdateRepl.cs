using OrleansGrainInterfaces.Device;

namespace ClusterClientConsole;

public static class UpdateRepl
{
    public static async Task StartReplAsync(IGrainFactory client)
    {
        var devices = new Dictionary<string, IDeviceGrain>();

        Console.WriteLine("IoT DeviceGrain REPL");

string[] commands =
[
    "create",
    "update",
    "status",
    "list",
    "load",
    "deactivate",
    "exit"
];
        while (true)
        {
            Console.Write("> ");
            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
                continue;

            var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
                continue;

            var command = parts[0].ToLower();

            switch (command)
            {
                case "h":
                    foreach(var commandOption in commands) Console.WriteLine($"  {commandOption}");
                    break;
                case "exit":
                    return;

                case "create":
                    if (parts.Length < 2)
                    {
                        Console.WriteLine("Usage: create <deviceId>");
                        break;
                    }
                    var id = parts[1];
                    if (!devices.ContainsKey(id))
                    {
                        var device = client.GetGrain<IDeviceGrain>(id);
                        devices[id] = device;
                        Console.WriteLine($"Device '{id}' created.");
                    }
                    else
                    {
                        Console.WriteLine($"Device '{id}' already exists.");
                    }
                    break;

                case "update":
                    if (parts.Length < 3)
                    {
                        Console.WriteLine("Usage: update <deviceId> <firmwareVersion>");
                        break;
                    }
                    id = parts[1];
                    if (devices.TryGetValue(id, out var updateDevice))
                    {
                        Console.WriteLine($"Updating '{id}' to version {parts[2]}...");
                        var success = await updateDevice.UpdateFirmware(parts[2]);
                        Console.WriteLine(success ? "Update succeeded." : "Update failed.");
                    }
                    else
                    {
                        Console.WriteLine($"Device '{id}' not found. Create it first.");
                    }
                    break;

                case "status":
                    if (parts.Length < 2)
                    {
                        Console.WriteLine("Usage: status <deviceId>");
                        break;
                    }
                    id = parts[1];
                    if (devices.TryGetValue(id, out var statusDevice))
                    {
                        var status = await statusDevice.GetStatus();
                        Console.WriteLine($"Device '{id}': {status}");
                    }
                    else
                    {
                        Console.WriteLine($"Device '{id}' not found.");
                    }
                    break;

                case "list":
                    if (devices.Count == 0)
                    {
                        Console.WriteLine("No devices created.");
                    }
                    else
                    {
                        foreach (var d in devices.Keys)
                            Console.WriteLine(d);
                    }
                    break;

                case "load":
                    if (parts.Length < 3 || !int.TryParse(parts[1], out var loadCount))
                    {
                        Console.WriteLine("Usage: load <count> <firmwareVersion> [concurrency]");
                        break;
                    }
                    var loadFirmware = parts[2];
                    int concurrency = 10;
                    if (parts.Length >= 4 && int.TryParse(parts[3], out var c))
                        concurrency = c;

                    Console.WriteLine($"Simulating {loadCount} concurrent updates to firmware {loadFirmware} (concurrency: {concurrency})...");

                    // Create grains if they don't already exist
                    for (int i = 0; i < loadCount; i++)
                    {
                        id = $"device_{i}";
                        if (!devices.ContainsKey(id))
                            devices[id] = client.GetGrain<IDeviceGrain>(id);
                    }

                    // Prepare tasks in batches
                    var deviceIds = devices.Keys.Take(loadCount).ToArray();
                    var updateTasks = new List<Task>();
                    var sw = System.Diagnostics.Stopwatch.StartNew();

                    for (int i = 0; i < deviceIds.Length; i += concurrency)
                    {
                        var batch = deviceIds.Skip(i).Take(concurrency)
                            .Select(id => devices[id].UpdateFirmware(loadFirmware));
                        updateTasks.AddRange(batch);
                        // Optionally: wait for current batch to complete before next
                        Console.WriteLine($"Batch {i / concurrency + 1} / {Math.Ceiling((double)deviceIds.Length / concurrency)} updating...");
                        await Task.WhenAll(batch);
                        Console.WriteLine($"Batch {i / concurrency + 1} / {Math.Ceiling((double)deviceIds.Length / concurrency)} complete...");
                    }

                    await Task.WhenAll(updateTasks);

                    sw.Stop();
                    Console.WriteLine($"Load simulation finished in {sw.Elapsed.TotalSeconds:F2} seconds.");

                    break;


                case "deactivate":
                    if (parts.Length < 2)
                    {
                        Console.WriteLine("Usage: deactivate <deviceId>");
                        break;
                    }
                    id = parts[1];
                    if (devices.TryGetValue(id, out var deactivateDevice))
                    {
                        await deactivateDevice.DeactivateAsync();
                        devices.Remove(id);
                        Console.WriteLine($"Device '{id}' deactivated.");
                    }
                    else
                    {
                        Console.WriteLine($"Device '{id}' not found.");
                    }
                    break;

                default:
                    Console.WriteLine("Unknown command.");
                    break;
            }
        }

    }
}
