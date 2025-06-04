using OrleansGrainInterfaces.Example; // Your grain interface

namespace ClusterClientConsole;

public static class GreetingGrainRepl
{
    public static async Task StartReplAsync(IGrainFactory client)
    {
        var greetingGrains = new List<IGreetingGrain>();

        Console.WriteLine("Welcome to the Orleans REPL.");

        string? line;
        while (true)
        {
            Console.WriteLine("Commands:");
            Console.WriteLine("  create <count>");
            Console.WriteLine("  get <index>");
            Console.WriteLine("  list");
            Console.WriteLine("  deactivate <index>");
            Console.WriteLine("  exit");
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
                    return;

                case "create":
                    if (tokens.Length < 2 || !int.TryParse(tokens[1], out var count) || count < 1)
                    {
                        Console.WriteLine("Usage: create <count>");
                        break;
                    }
                    for (int i = 0; i < count; i++)
                    {
                        var grain = client.GetGrain<IGreetingGrain>($"Mike: {greetingGrains.Count + i}");
                        greetingGrains.Add(grain);
                    }
                    Console.WriteLine($"Created {count} grains.");
                    break;

                case "get":
                    if (tokens.Length < 2 || !int.TryParse(tokens[1], out var getIndex) || getIndex < 0 || getIndex >= greetingGrains.Count)
                    {
                        Console.WriteLine("Usage: get <index>");
                        break;
                    }
                    var value = await greetingGrains[getIndex].GetAsync();
                    Console.WriteLine($"Grain[{getIndex}] => {value}");
                    break;

                case "list":
                    if (greetingGrains.Count == 0)
                    {
                        Console.WriteLine("No grains cached.");
                    }
                    else
                    {
                        for (int i = 0; i < greetingGrains.Count; i++)
                            Console.WriteLine($"[{i}] {await greetingGrains[i].GetAsync()}");
                    }
                    break;

                case "deactivate":
                    if (tokens.Length < 2 || !int.TryParse(tokens[1], out var killIndex) || killIndex < 0 || killIndex >= greetingGrains.Count)
                    {
                        Console.WriteLine("Usage: deactivate <index>");
                        break;
                    }
                    await greetingGrains[killIndex].DeactivateAsync();
                    Console.WriteLine($"Grain[{killIndex}] deactivated.");
                    break;

                default:
                    Console.WriteLine("Unknown command.");
                    break;
            }
        }
    }
}
