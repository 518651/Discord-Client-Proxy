using System.Diagnostics;
using System.Text.Json;
using DiscordClientProxy.Interfaces;

namespace DiscordClientProxy.StartupTasks;

public class LaunchProcessOnFinishTask : IStartupTask
{
    public int Order { get; } = 99;
    public Task ExecuteAsync()
    {
        if(Configuration.Instance.Debug.SpawnProcessOnReady is not null)
        {
            Directory.CreateDirectory(Configuration.Instance.AssetCacheLocationResolved+"/.vscode");
            File.WriteAllText(Configuration.Instance.AssetCacheLocationResolved+"/.vscode/settings.json", "{\"editor.codeLens\":false}");
            
            var cmd = ParseArguments(Configuration.Instance.Debug.SpawnProcessOnReady.Replace("$DIR", Configuration.Instance.AssetCacheLocationResolved));
            Console.WriteLine($"Launching process: {JsonSerializer.Serialize(cmd)}");
            Process.Start(cmd[0], cmd[1..]);
        }
        
        return Task.CompletedTask;
    }
    
    // source: https://stackoverflow.com/a/298968
    static string[] ParseArguments(string commandLine)
    {
        char[] parmChars = commandLine.ToCharArray();
        bool inQuote = false;
        for (int index = 0; index < parmChars.Length; index++)
        {
            if (parmChars[index] == '"')
                inQuote = !inQuote;
            if (!inQuote && parmChars[index] == ' ')
                parmChars[index] = '\n';
        }
        return new string(parmChars).Split('\n');
    }
}