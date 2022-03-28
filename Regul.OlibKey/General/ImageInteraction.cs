using System;
using System.Net;
using System.Threading.Tasks;

namespace Regul.OlibKey.General;

public static class ImageInteraction
{
    public static async Task<string?> DownloadImage(string url, int size = 16)
    {
        using WebClient client = new();
        
        if (url.Contains("http://") || url.Contains("https://"))
        {
            return Convert.ToBase64String(await client.DownloadDataTaskAsync(
                $"https://t3.gstatic.com/faviconV2?client=SOCIAL&type=FAVICON&fallback_opts=TYPE,SIZE,URL&url={url}&size={size}"));
        }
                
        return Convert.ToBase64String(await client.DownloadDataTaskAsync(
            $"https://t3.gstatic.com/faviconV2?client=SOCIAL&type=FAVICON&fallback_opts=TYPE,SIZE,URL&url=https://{url}&size={size}"));
    }
}