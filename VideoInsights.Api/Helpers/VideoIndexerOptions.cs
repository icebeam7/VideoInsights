using System.ComponentModel.DataAnnotations;

namespace VideoInsights.Api.Helpers
{
    public class VideoIndexerOptions
    {
        [Required]
        public string SubscriptionKey { get; set; }

        [Required]
        public string AccountId { get; set; }

        [Required]
        public string Location { get; set; }
    }
}