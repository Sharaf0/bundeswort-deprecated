using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bundeswort.Scraper
{
    public interface IClient
    {
        Task<Dictionary<string, string>> GetCaptionsUrls(string videoId, string[] lang);
        Task<string> GetCaptionXmlContent(string value);
    }
}