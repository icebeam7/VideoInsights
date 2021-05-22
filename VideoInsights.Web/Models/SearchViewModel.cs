using System.Collections.Generic;

using VideoInsights.Models;

namespace VideoInsights.Web.Models
{
    public class SearchViewModel
    {
        public string Text { get; set; }

        public List<VideoData> Videos { get; set; }
    }
}
