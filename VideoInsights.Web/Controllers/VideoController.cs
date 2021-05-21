using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using VideoInsights.Models;
using VideoInsights.Web.Models;
using VideoInsights.Web.Helpers;
using System.Net.Http.Headers;

namespace VideoInsights.Web.Controllers
{
    public class VideoController : Controller
    {
        private readonly HttpClient _client;
        private readonly VideoOptions _options;

        private readonly string videoIndexerApiUrl = "api/VideoIndexer";
        private readonly string videoDbApiUrl = "api/VideoDb";

        public VideoController(IOptions<VideoOptions> videoOptions)
        {
            _options = videoOptions.Value;
            _client = HttpClientHelper.CreateInstance(_options.Url);
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var response = await _client.GetAsync(videoIndexerApiUrl);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var videoData = JsonConvert.DeserializeObject<List<VideoData>>(json);

                foreach (var item in videoData)
                    item.Thumbnail.Content = ThumbnailHelper.AddData(item.Thumbnail);

                return View(videoData);
            }

            return View(default(VideoData));
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            var url = $"{videoIndexerApiUrl}/{id}";
            var response = await _client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var videoDetailsData = JsonConvert.DeserializeObject<VideoDetailsData>(json);

                foreach (var item in videoDetailsData.KeyFrames)
                    item.Thumbnail.Content = ThumbnailHelper.AddData(item.Thumbnail);

                return View(videoDetailsData);
            }

            return View(default(VideoDetailsData));
        }

        [HttpGet]
        [ActionName("Upload")]
        public IActionResult UploadView(UploadViewModel vm)
        {
            if (vm == null)
                vm = new UploadViewModel();

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Upload(UploadViewModel vm)
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
            return UploadView(vm);
        }

        [HttpGet]
        public async Task<IActionResult> IndexDb()
        {
            var response = await _client.GetAsync(videoDbApiUrl);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var videos = JsonConvert.DeserializeObject<List<Video>>(json);
                return View(videos);
            }

            return View(default(Video));
        }
    }
}