using System.ComponentModel.DataAnnotations;

namespace VideoInsights.Web.Helpers
{
    public class VideoOptions
    {
        [Required]
        public string Url { get; set; }
    }
}