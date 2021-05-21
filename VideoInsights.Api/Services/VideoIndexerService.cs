using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

using VideoInsights.Models;
using VideoInsights.Api.Helpers;

namespace VideoInsights.Api.Services
{
    public class VideoIndexerService : IVideoIndexerService
    {
        private readonly HttpClient _client;
        private readonly VideoIndexerOptions _options;
        private readonly string _videoIndexerBaseUrl = $"https://api.videoindexer.ai/";
        private string _videoIndexerAccessToken = string.Empty;

        public VideoIndexerService(IOptions<VideoIndexerOptions> videoIndexerOptions)
        {
            _options = videoIndexerOptions.Value;
            _client = HttpClientHelper.CreateInstance(_videoIndexerBaseUrl, _options.SubscriptionKey);
        }

        public async Task<bool> UploadVideo(IFormFile file)
        {
            await GetToken();

            //var localFilePath = await WriteFile(file);

            using var stream = file.OpenReadStream();
            var byteData = FileHelper.GetBytes(stream);

            using MultipartFormDataContent content = new MultipartFormDataContent
            {
                new ByteArrayContent(byteData)
            };

            var url = $"{_options.Location}/Accounts/{_options.AccountId}" +
                $"/Videos?privacy=Public" +
                $"&accessToken={_videoIndexerAccessToken}" +
                $"&name={file.FileName}";

            var response = await _client.PostAsync(url, content);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<VideoData>> GetVideos()
        {
            await GetToken();

            var url = $"{_options.Location}/Accounts/{_options.AccountId}" +
                $"/Videos" +
                $"?accessToken={_videoIndexerAccessToken}";

            var response = await _client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var videoResult = JsonConvert.DeserializeObject<VideoResultData>(json);

                if (videoResult.Results.Count > 0)
                {
                    var videos = new List<VideoData>();

                    foreach (var item in videoResult.Results)
                    {
                        var thumbnail = await GetThumbnail(item.Id, item.ThumbnailId);

                        videos.Add(new VideoData()
                        {
                            Id = item.Id,
                            Name = item.Name,
                            Created = item.Created,
                            LastIndexed = item.LastIndexed,
                            Thumbnail = new ThumbnailData()
                            {
                                Id = item.ThumbnailId,
                                Content = thumbnail
                            },
                        });
                    }

                    return videos;
                }
            }

            return default;
        }

        public async Task<VideoDetailsData> GetVideoDetails(string id)
        {
            await GetToken();

            var details = new VideoDetailsData
            {
                Id = id, 
                Uri = await GetVideoUrl(id),
                KeyFrames = new List<KeyFrameData>()
            };

            var url = $"{_options.Location}/Accounts/{_options.AccountId}" +
                $"/Videos/{id}" +
                $"/Index?reTranslate=False&includeStreamingUrls=True" +
                $"&accessToken={_videoIndexerAccessToken}";

            var labels = new List<LabelInsightsData>();
            var indexResponse = await _client.GetAsync(url);

            if (indexResponse.IsSuccessStatusCode)
            {
                var indexContent = await indexResponse.Content.ReadAsStringAsync();
                var videoIndex = JsonConvert.DeserializeObject<VideoIndexData>(indexContent);

                if (videoIndex != null)
                {
                    details.Name = videoIndex.Name;
                    var video = videoIndex.Videos.FirstOrDefault();

                    if (video != null)
                    {
                        foreach (var shot in video.Insights.Shots)
                            foreach (var keyFrame in shot.KeyFrames)
                                foreach (var instance in keyFrame.Instances)
                                {
                                    var thumbnail = await GetThumbnail(id, instance.ThumbnailId);

                                    details.KeyFrames.Add(new KeyFrameData()
                                    {
                                        TimeData = new TimeData()
                                        {
                                            Start = instance.Start,
                                            End = instance.End
                                        },
                                        Thumbnail = new ThumbnailData()
                                        {
                                            Id = instance.ThumbnailId,
                                            Content = thumbnail
                                        }
                                    });
                                }

                        labels = video.Insights.Labels.ToList();
                    }
                }
            }

            var labelDetails = new List<LabelDetailsData>();

            foreach (var item in labels)
            {
                var ac = new List<AppearanceData>();

                if (item.Instances != null)
                {
                    foreach (var app in item.Instances)
                    {
                        ac.Add(new AppearanceData()
                        {
                            StartTime = TimeHelper.ConvertTime(app.Start),
                            EndTime = TimeHelper.ConvertTime(app.End)
                        });
                    }
                }

                labelDetails.Add(new LabelDetailsData()
                {
                    Name = item.Name,
                    Appearances = ac
                });
            }

            foreach (var item in details.KeyFrames)
            {
                item.Labels = new List<LabelData>();
                var startTime = TimeHelper.ConvertTime(item.TimeData.Start);
                var endTime = TimeHelper.ConvertTime(item.TimeData.End);

                foreach (var labelDetail in labelDetails)
                {
                    if (labelDetail.Appearances.Any(x =>
                            startTime >= x.StartTime && endTime <= x.EndTime))
                        item.Labels.Add(new LabelData() { Label = labelDetail.Name });
                }
            }

            return details;
        }

        private async Task<string> GetVideoUrl(string id)
        {
            var url = $"{_options.Location}/Accounts/{_options.AccountId}" +
                $"/Videos/{id}" +
                $"/SourceFile/DownloadUrl" +
                $"?accessToken={_videoIndexerAccessToken}";

            var downloadUriResponse = await _client.GetAsync(url);

            if (downloadUriResponse.IsSuccessStatusCode)
            {
                var videoUrl = await downloadUriResponse.Content.ReadAsStringAsync();
                //return videoUrl.Substring(1, videoUrl.Length - 2);
                return videoUrl[1..^1];
            }

            return string.Empty;
        }

        private async Task<string> GetThumbnail(string videoId, string thumbnailId)
        {
            await GetToken();

            var url = $"{_options.Location}/Accounts/{_options.AccountId}" +
                $"/Videos/{videoId}" +
                $"/Thumbnails/{thumbnailId}" +
                $"?format=Base64" +
                $"&accessToken={_videoIndexerAccessToken}";

            var thumbnailResponse = await _client.GetAsync(url);

            return thumbnailResponse.IsSuccessStatusCode
                ? await thumbnailResponse.Content.ReadAsStringAsync()
                : string.Empty;
        }

        private async Task GetToken()
        {
            if (string.IsNullOrWhiteSpace(_videoIndexerAccessToken))
            {
                var url = $"Auth/{_options.Location}/Accounts/{_options.AccountId}" +
                    $"/AccessToken?allowEdit=True";

                var tokenResponse = await _client.GetAsync(url);

                if (tokenResponse.IsSuccessStatusCode)
                {
                    var content = await tokenResponse.Content.ReadAsStringAsync();
                    _videoIndexerAccessToken = content[1..^1];
                }
            }
        }
    }
}