namespace DiscordClientProxy.Classes;

public abstract class ClientPatch
{
    public virtual bool IsEnabledByDefault { get; set; } = true;

    public string GetPrefix()
    {
        return $"[ClientPatcher:{GetType().Name.Replace("Patch", "")}]";
    }
    
    public async Task ApplyPatchToFile(string filePath)
    {
        Console.WriteLine($"{GetPrefix()} Applying patch to {filePath}...");
        await File.WriteAllTextAsync(filePath, await ApplyPatch(await File.ReadAllTextAsync(filePath)));
    }

    public virtual async Task<string> ApplyPatch(string content)
    {
        Console.WriteLine($"{GetPrefix()} Not implemented!");
        return content;
    }
}