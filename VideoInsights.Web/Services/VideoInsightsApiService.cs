using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Collections.Generic;

using Newtonsoft.Json;

using VideoInsights.Models;
using VideoInsights.Web.Models;
using VideoInsights.Web.Helpers;

namespace VideoInsights.Web.Services
{
    public class VideoInsightsApiService
    {
        private readonly HttpClient _client;

        private readonly string videoIndexerApiUrl = "api/VideoIndexer";
        private readonly string videoDbApiUrl = "api/VideoDb";
        private readonly string labelDbApiUrl = "api/LabelDb";
        private readonly string keyframeDbApiUrl = "api/KeyframeDb";
        private readonly string thumbnailDbApiUrl = "api/ThumbnailDb";
        private readonly string timeperiodDbApiUrl = "api/TimeperiodDb";

        public VideoInsightsApiService(string url)
        {
            _client = HttpClientHelper.CreateInstance(url);
        }

        public async Task<List<VideoData>> GetVideosIndexerData()
        {
            var response = await _client.GetAsync(videoIndexerApiUrl);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var videoData = JsonConvert.DeserializeObject<List<VideoData>>(json);

                foreach (var item in videoData)
                    item.Thumbnail.Content = ThumbnailHelper.AddData(item.Thumbnail);

                return videoData;
            }

            return default;
        }

        public async Task<DetailsViewModel> GetDetails(string id)
        {
            var vm = new DetailsViewModel();

            var url = $"{videoIndexerApiUrl}/{id}";
            var response = await _client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var videoDetailsData = JsonConvert.DeserializeObject<VideoDetailsData>(json);

                foreach (var item in videoDetailsData.KeyFrames)
                    item.Thumbnail.Content = ThumbnailHelper.AddData(item.Thumbnail);

                vm.VideoDetailsData = videoDetailsData;

                var urlDb = $"{videoDbApiUrl}/{id}";
                var responseDb = await _client.GetAsync(urlDb);
                vm.IsNew = !responseDb.IsSuccessStatusCode;
            }
            else
                vm.VideoDetailsData = new VideoDetailsData();

            return vm;
        }

        public async Task<UploadViewModel> UploadVideo(UploadViewModel vm)
        {
            var file = vm.VideoFile;
            var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');

            using MultipartFormDataContent content = new MultipartFormDataContent();
            content.Add(new StreamContent(file.OpenReadStream())
            {
                Headers =
                {
                    ContentLength = file.Length,
                    ContentType = new MediaTypeHeaderValue(file.ContentType)
                }
            }, "File", fileName);

            var response = await _client.PostAsync(videoIndexerApiUrl, content);
            vm.Result = response.IsSuccessStatusCode;
            return vm;
        }

        private async Task<bool> InsertDb(string service, string json)
        {
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(service, content);
            return response.IsSuccessStatusCode;
        }

        private async Task<bool> InsertVideo(VideoDetailsData videoData)
        {
            var videos = await GetVideosIndexerData();
            var video = videos.First(x => x.Id == videoData.Id);

            var videoDb = new Video()
            {
                Id = videoData.Id,
                Name = videoData.Name,
                Uri = videoData.Uri,
                Created = videoData.Created,
                LastIndexed = video.LastIndexed,
            };

            var json = JsonConvert.SerializeObject(videoDb);
            var insertVideo = await InsertDb(videoDbApiUrl, json);

            if (insertVideo)
            {
                var thumbnail = video.Thumbnail;
                await InsertThumbnail(thumbnail, video.Id);
            }

            return insertVideo;
        }

        private async Task<bool> InsertKeyFrame(KeyFrameData keyframe, string videoId)
        {
            var keyframeDb = new KeyFrame()
            {
                Id = keyframe.Id,
                VideoId = videoId
            };

            var json = JsonConvert.SerializeObject(keyframeDb);
            return await InsertDb(keyframeDbApiUrl, json);
        }

        private async Task<bool> InsertLabel(LabelData label, string keyframeId)
        {
            var labelDb = new Label()
            {
                Id = label.Id,
                KeyframeId = keyframeId,
                Content = label.Label
            };

            var json = JsonConvert.SerializeObject(labelDb);
            return await InsertDb(labelDbApiUrl, json);
        }

        private async Task<bool> InsertTimePeriod(TimeData time, string keyframeId)
        {
            var timeDb = new TimePeriod()
            {
                Id = time.Id,
                KeyframeId = keyframeId,
                EndTime = time.End,
                StartTime = time.Start
            };

            var json = JsonConvert.SerializeObject(timeDb);
            return await InsertDb(timeperiodDbApiUrl, json);
        }

        private async Task<bool> InsertThumbnail(ThumbnailData thumbnail, string externalId)
        {
            var thumbnailDb = new Thumbnail()
            {
                Id = thumbnail.Id,
                ExternalId = externalId,
                Content = thumbnail.Content
            };

            var json = JsonConvert.SerializeObject(thumbnailDb);
            return await InsertDb(thumbnailDbApiUrl, json);
        }

        public async Task<DetailsViewModel> AddDb(string id)
        {
            var vm = await GetDetails(id);

            var videoData = vm.VideoDetailsData;
            var insertVideo = await InsertVideo(videoData);

            if (insertVideo)
            {
                var keyframes = vm.VideoDetailsData.KeyFrames;

                foreach (var keyframe in keyframes)
                {
                    var insertKeyframe = await InsertKeyFrame(keyframe, id);

                    if (insertKeyframe)
                    {
                        var labels = keyframe.Labels;

                        foreach (var label in labels)
                            await InsertLabel(label, keyframe.Id);

                        var timeperiod = keyframe.TimeData;
                        await InsertTimePeriod(timeperiod, keyframe.Id);

                        var thumbnail = keyframe.Thumbnail;
                        await InsertThumbnail(thumbnail, keyframe.Id);
                    }
                }

                vm.IsNew = false;
            }

            return vm;
        }

        public async Task<List<Video>> GetVideosDb()
        {
            var response = await _client.GetAsync(videoDbApiUrl);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var videos = JsonConvert.DeserializeObject<List<Video>>(json);

                foreach (var video in videos)
                {
                    var urlT = $"{thumbnailDbApiUrl}/ByExternal/{video.Id}";
                    var responseT = await _client.GetAsync(urlT);

                    if (responseT.IsSuccessStatusCode)
                    {
                        var jsonT = await responseT.Content.ReadAsStringAsync();
                        var thumbnails = JsonConvert.DeserializeObject<List<Thumbnail>>(jsonT);
                        video.Thumbnail = thumbnails;
                    }
                }

                return videos;
            }

            return default;
        }

        public async Task<DetailsDbViewModel> GetDetailsDb(string id)
        {
            var vm = new DetailsDbViewModel();

            var url = $"{videoDbApiUrl}/{id}";
            var response = await _client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var videoDetailsData = JsonConvert.DeserializeObject<Video>(json);

                var urlK = $"{keyframeDbApiUrl}/ByVideo/{id}";
                var responseK = await _client.GetAsync(urlK);

                if (responseK.IsSuccessStatusCode)
                {
                    var jsonK = await responseK.Content.ReadAsStringAsync();
                    var keyframes = JsonConvert.DeserializeObject<List<KeyFrame>>(jsonK);
                    videoDetailsData.KeyFrameList = keyframes;

                    foreach (var keyframe in keyframes)
                    {
                        var urlL = $"{labelDbApiUrl}/ByKeyframe/{keyframe.Id}";
                        var responseL = await _client.GetAsync(urlL);

                        if (responseL.IsSuccessStatusCode)
                        {
                            var jsonL = await responseL.Content.ReadAsStringAsync();
                            var labels = JsonConvert.DeserializeObject<List<Label>>(jsonL);
                            keyframe.Labels = labels;
                        }

                        var urlT = $"{thumbnailDbApiUrl}/ByExternal/{keyframe.Id}";
                        var responseT = await _client.GetAsync(urlT);

                        if (responseT.IsSuccessStatusCode)
                        {
                            var jsonT = await responseT.Content.ReadAsStringAsync();
                            var thumbnails = JsonConvert.DeserializeObject<List<Thumbnail>>(jsonT);
                            keyframe.Thumbnail = thumbnails;
                        }

                        var urlTP = $"{timeperiodDbApiUrl}/ByKeyframe/{keyframe.Id}";
                        var responseTP = await _client.GetAsync(urlTP);

                        if (responseTP.IsSuccessStatusCode)
                        {
                            var jsonTP = await responseTP.Content.ReadAsStringAsync();
                            var timeperiods = JsonConvert.DeserializeObject<List<TimePeriod>>(jsonTP);
                            keyframe.TimePeriod = timeperiods;
                        }
                    }

                    vm.VideoDetailsData = videoDetailsData;
                }
            }
            else
                vm.VideoDetailsData = new Video();

            return vm;
        }

    }
}
