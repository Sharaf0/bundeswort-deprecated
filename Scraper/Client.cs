using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json.Linq;

namespace Bundeswort.Scraper
{
    public class Client : IClient
    {
        const string YoutubeVideoTemplate = "https://www.youtube.com/watch?v={0}";
        const string YoutubeVideoInfo = "https://youtube.com/get_video_info?video_id={0}";
        const string PlayerResponse = "player_response";
        //
        // Summary:
        //     Get Dictionary of languages and their corresponding captions urls
        //
        // Parameters:
        //   videoId:
        //     Youtube videoId
        //
        // Todos:
        //     Dont't get just one url per language / culture
        public async Task<Dictionary<string, string>> GetCaptionsUrls(string videoId, string[] langs)
        {
            Console.WriteLine("Getting captions URLs");

            HttpClient client = new HttpClient();
            var res = await client.GetAsync(string.Format(YoutubeVideoInfo, videoId));
            var content = await res.Content.ReadAsStringAsync();

            if (!getValues(content).TryGetValue("player_response", out string playerResponse))
                throw new Exception("Video is not valid");

            if (JObject.Parse(playerResponse)["playabilityStatus"]["status"].ToString() != "OK")
                throw new Exception("Video is not valid");

            var temp = JObject.Parse(playerResponse)["captions"]["playerCaptionsTracklistRenderer"]["captionTracks"];

            var captionTracks = JObject.Parse(playerResponse)["captions"]["playerCaptionsTracklistRenderer"]["captionTracks"]
            //.Where(ct => ct["kind"] == null || ct["kind"].ToString() != "asr")
            .Where(ct => langs.Any(l => l == ct["languageCode"].ToString().Substring(0, 2).ToLower()))
            .ToArray()
            .Select(ct => KeyValuePair.Create(ct["languageCode"].ToString().Substring(0, 2), ct["baseUrl"].ToString()))
            .GroupBy(kv => kv.Key, kv => kv.Value, (key, g) => KeyValuePair.Create(key, g.ToList()))
            .ToList();

            Console.WriteLine($"Received URLs for those languages {string.Join(',', captionTracks.Select(ct => ct.Key))}");

            var dict = new Dictionary<string, string>(captionTracks.Select(ct => KeyValuePair.Create(ct.Key, ct.Value.First())));
            return dict;
        }

        public async Task<string> GetCaptionXmlContent(string url)
        {
            try
            {
                HttpClient client = new HttpClient();
                var res = await client.GetAsync(url);
                var content = await res.Content.ReadAsStringAsync();
                return content;
            }
            catch (System.Exception e)
            {
                return e.Message;
            }
        }

        private Dictionary<string, string> getValues(string content)
        {
            var ret = new Dictionary<string, string>();

            var queries = content.Split('&');
            foreach (var query in queries)
            {
                var values = query.Split('=');
                var key = values[0];
                var value = values[1];
                var decodedValue = HttpUtility.UrlDecode(value);
                ret.Add(key, decodedValue);
            }
            return ret;
        }
    }
}