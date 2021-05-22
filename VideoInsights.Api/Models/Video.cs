using System;
using System.Collections.Generic;

#nullable disable

namespace VideoInsights.Api.Models
{
    public partial class Video
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Uri { get; set; }
        public DateTime Created { get; set; }
        public DateTime Lastindexed { get; set; }
    }
}
