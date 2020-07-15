using System.Collections.Generic;

namespace Bundeswort.Scraper
{
    public class VideoCaptionParts
    {
        public string VideoId { get; set; }
        public IEnumerable<CaptionPart> CaptionParts { get; set; }
    }
}