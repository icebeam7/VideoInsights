using System;
using System.Collections.Generic;

namespace VideoInsights.Models
{
    public class Video
    {
        public int Id { get; set; }
        public string VideoIndexerId { get; set; }
        public string Name { get; set; }
        public string Uri { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastIndexed { get; set; }
        public List<Thumbnail> Thumbnail { get; set; }
        public List<KeyFrame> KeyFrameList { get; set; }
    }

    public class Thumbnail
    {
        public string Id { get; set; }
        public string Content { get; set; }
    }

    public class KeyFrame
    {
        public string Id { get; set; }
        public List<TimePeriod> TimePeriod { get; set; }
        public List<Thumbnail> Thumbnail { get; set; }
        public List<Label> Labels { get; set; }
    }

    public class TimePeriod
    {
        public string Id { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
    }

    public class Label
    {
        public string Id { get; set; }
        public string Content { get; set; }
    }
}