using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Bundeswort.Scraper
{
    public class CaptionsScraper
    {
        private readonly IClient client;

        public CaptionsScraper(IClient client)
        {
            this.client = client;
        }

        public async Task<List<VideoCaption>> Scrap(string videoId, string[] langs)
        {
            if (string.IsNullOrWhiteSpace(videoId))
                throw new ArgumentException("VideoId cannot be null or empty");

            try
            {
                List<VideoCaption> lst = new List<VideoCaption>();
                var urlsDictionary = await client.GetCaptionsUrls(videoId, langs);
                foreach ((string language, string url) in urlsDictionary)
                {
                    Console.WriteLine($"Getting captions for{videoId}-{language}");

                    string xmlContent = await client.GetCaptionXmlContent(url);
                    (string fullText, List<CaptionPart> cps) = GetCaptionParts(xmlContent);
                    lst.Add(new VideoCaption
                    {
                        CaptionParts = cps,
                        Language = language
                    });

                    Console.WriteLine($"Getting captions for {videoId}-{language} | Done!");
                }
                return lst;
            }
            catch (System.Exception)
            {
                return new List<VideoCaption>();
            }

        }

        (string, List<CaptionPart>) GetCaptionParts(string xmlContent)
        {
            XElement xElement = XElement.Parse(xmlContent);
            var elements = xElement.Elements().ToList();
            var items = elements.Select(e => new CaptionPart(e)).ToList();
            var fullText = items.Select((e, i) => $"||{i}||{e.Text}").Aggregate((a, b) => a + b);
            return (fullText, items);
        }
    }

}