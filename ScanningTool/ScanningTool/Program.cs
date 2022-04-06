var path = Environment.GetEnvironmentVariable("GITHUB_EVENT_PATH");
string jsonString;
using (var reader = new StreamReader(path))
{
    jsonString = reader.ReadToEnd();
}
Console.WriteLine(jsonString);
return;
//var client = new HttpClient();
//await client.GetAsync($"https://api.github.com/repos/{Environment.GetEnvironmentVariable("GITHUB_REPOSITORY")}/pulls/{pr_num}/files");
Console.WriteLine(Environment.GetEnvironmentVariable("GITHUB_REPOSITORY"));
return;
if (args.Length < 1)
{
    Console.WriteLine("There are no args, exit.");
    return;
}

switch (args[0])
{
    case "help":
        if (args.Length == 1)
        {
            Console.WriteLine("some information.");
            break;
        }

        Console.WriteLine("Help does not allow parameters.");
        break;
    default:
        Console.WriteLine("Given args:");
        foreach (var arg in args)
        {
            Console.WriteLine(arg);
        }

        break;
}
