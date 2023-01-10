using System.Collections.Concurrent;
using System.Text;
using DiscordClientProxy.Utilities;

namespace DiscordClientProxy;

public class MemoryStore
{
    public static readonly ConcurrentDictionary<string, byte[]> asset_cache = new();
    public static readonly ConcurrentDictionary<string, byte[]> resource_cache = new();
    public static byte[]? ClientPageHtml { get; set; }
    public static byte[]? DevPageHtml { get; set; }
    
    public static List<string> ClientPreloadScripts { get; set; } = new();
    public static List<string> ClientStylesheets { get; set; } = new();
    public static List<string> ClientScripts { get; set; } = new();
    
    public static List<string> DevPreloadScripts { get; set; } = new();
    public static List<string> DevStylesheets { get; set; } = new();
    public static List<string> DevScripts { get; set; } = new();
}

public class TieredAssetStore
{
    public static bool RecordNewDownloads = false;
    public static async Task<byte[]> GetAsset(string asset)
    {
        //ensure no asset prefix
        asset = asset.Replace("/assets/", "");
        
        byte[] data;
        if (Configuration.Instance.Cache.Memory && MemoryStore.asset_cache.TryGetValue(asset, out data)) return data;
        if (Configuration.Instance.Cache.Disk && File.Exists(Configuration.Instance.AssetCacheLocationResolved + asset))
            data = await File.ReadAllBytesAsync(Configuration.Instance.AssetCacheLocationResolved + asset);
        else
        {
            using HttpClient client = new();
            Console.WriteLine($"Downloading asset: {Configuration.Instance.Cache.AssetBaseUri + asset}");
            data = await client.GetByteArrayAsync(Configuration.Instance.Cache.AssetBaseUri + asset);
            if (asset.EndsWith(".js") || asset.EndsWith(".css"))
                data = Encoding.UTF8.GetBytes(
                    await ClientPatcher.Patch(
                        Encoding.UTF8.GetString(data)
                    )
                );
            if(RecordNewDownloads)
                await File.AppendAllTextAsync("cache_misses", $"{asset}\n");
        }

        await PromoteAsset(asset, data);
        return data;
    }

    public static List<string> EnumerateAssets()
    {
        var entries = new List<string>();
        entries.AddRange(MemoryStore.asset_cache.Keys);
        entries.AddRange(Directory.EnumerateFiles(Configuration.Instance.AssetCacheLocationResolved).Select(x=>new FileInfo(x).Name));
        return entries.Distinct().ToList();
    }

    private static async Task PromoteAsset(string asset, byte[] data)
    {
        if (Configuration.Instance.Cache.Memory && !MemoryStore.asset_cache.ContainsKey(asset))
            MemoryStore.asset_cache.TryAdd(asset, data);
        if (Configuration.Instance.Cache.Disk && !File.Exists(Configuration.Instance.AssetCacheLocationResolved + asset)) 
            await File.WriteAllBytesAsync(Configuration.Instance.AssetCacheLocationResolved + asset, data);
    }
}