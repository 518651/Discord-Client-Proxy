using System.Text.RegularExpressions;
using DiscordClientProxy.Classes;

namespace DiscordClientProxy.ClientPatches;

public class RemoveModalsPatch : ClientPatch
{
    public override async Task<string> ApplyPatch(string content)
    {
        //if (!content.Contains("delete window.localStorage")) return content;
        Console.WriteLine($"{GetPrefix()} Applying patch...");
        //content = Regex.Replace(content, @"\w\?\(\d,\w\.jsx\)\(\w*\,{authTokenCallback:this\.handleAuthToken}\):null", "null");
        content = Regex.Replace(content, @" return .\.showPremiumUpsell", "return false");
        content = Regex.Replace(content, @".\.showUpsellModal\(.\)", "console.log('Prevented modal!')");
        content = content.Replace("showPremiumCTA:", "showPremiumCTA:false&&");
        content = content.Replace("showRoleSubscriptionCTA:", "showRoleSubscriptionCTA:false&&");
        content = Regex.Replace(content, @"=.\.shouldShowUpsells", "= false");
        return content;
    }
}