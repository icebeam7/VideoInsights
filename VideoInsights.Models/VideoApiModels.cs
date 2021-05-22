using System;
using System.Collections.Generic;

namespace VideoInsights.Models
{
    public class VideoResultData
    {
        public List<ResultData> Results { get; set; }
    }

    public class ResultData
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastIndexed { get; set; }
        public int DurationInSeconds { get; set; }
        public string ThumbnailVideoId { get; set; }
        public string ThumbnailId { get; set; }
    }

    public class VideoIndexData
    {
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastIndexed { get; set; }
        public IList<VideoInsightsData> Videos { get; set; }
    }

    public class VideoInsightsData
    {
        public InsightsData Insights { get; set; }
    }

    public class InsightsData
    {
        public IList<LabelInsightsData> Labels { get; set; }
        public IList<ShotData> Shots { get; set; }
    }

    public class LabelInsightsData
    {
        public string Name { get; set; }
        public IList<InstanceData> Instances { get; set; }
    }

    public class ShotData
    {
        public IList<KeyFrameDetailsData> KeyFrames { get; set; }
    }

    public class InstanceData
    {
        public string Start { get; set; }
        public string End { get; set; }
        public string Duration { get; set; }
        public string ThumbnailId { get; set; }
    }

    public class KeyFrameDetailsData
    {
        public string Id { get; set; }
        public IList<InstanceData> Instances { get; set; }
    }

    public class LabelDetailsData
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public IList<AppearanceData> Appearances { get; set; }
    }

    public class AppearanceData
    {
        public double StartTime { get; set; }
        public double EndTime { get; set; }
    }

    public class VideoData
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public ThumbnailData Thumbnail { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastIndexed { get; set; }
    }

    public class ThumbnailData
    {
        public string Id { get; set; }
        public string Content { get; set; }
    }

    public class VideoDetailsData
    {
        public string Id { get; set; }
        public string Uri { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastIndexed { get; set; }

        public List<KeyFrameData> KeyFrames { get; set; }
    }

    public class KeyFrameData
    {
        public string Id { get; set; }
        public TimeData TimeData { get; set; }
        public ThumbnailData Thumbnail { get; set; }
        public List<LabelData> Labels { get; set; }
    }

    public class TimeData
    {
        public string Id { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
    }

    public class LabelData
    {
        public string Id { get; set; }
        public string Label { get; set; }
    }
}