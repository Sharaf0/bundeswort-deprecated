using System.Web;
using System.Xml.Linq;

namespace Bundeswort.Scraper
{
    public class CaptionPart
    {
        public double Start { get; set; }
        public double Duration { get; set; }
        public string Text { get; set; }
        public CaptionPart(XElement e)
        {
            Start = double.Parse(e.Attribute("start").Value);
            Duration = double.Parse(e.Attribute("dur").Value);
            Text = HttpUtility.HtmlDecode(e.Value).Replace("\n", string.Empty);
        }
    }
}