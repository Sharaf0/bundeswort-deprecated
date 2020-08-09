using System.ComponentModel.DataAnnotations;

namespace Bundeswort.Controllers
{
    public class ChannelDetails
    {
        [Required]
        public string ChannelId { get; set; }
        [Required]
        public string Language { get; set; }
    }
}
