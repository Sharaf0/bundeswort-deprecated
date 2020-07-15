using System.Collections.Generic;

namespace Bundeswort.Scraper
{
    public class VideoCaption
    {
        public string Language { get; set; }
        public string FullText { get; set; }
        public List<CaptionPart> CaptionParts { get; set; }
    }
}