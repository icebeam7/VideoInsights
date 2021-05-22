using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using VideoInsights.Web.Models;
using VideoInsights.Web.Helpers;
using VideoInsights.Web.Services;

namespace VideoInsights.Web.Controllers
{
    public class VideoController : Controller
    {
        private readonly VideoOptions _options;
        private VideoInsightsApiService _service;

        public VideoController(IOptions<VideoOptions> videoOptions)
        {
            _options = videoOptions.Value;
            _service = new VideoInsightsApiService(_options.Url);
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var data = await _service.GetVideosIndexerData();
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            var vm = await _service.GetDetails(id);
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> DetailsDb(string id)
        {
            var vm = await _service.GetDetailsDb(id);
            return View(vm);
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
            var vmU = await _service.UploadVideo(vm);
            return UploadView(vmU);
        }


        [HttpPost]
        public async Task<IActionResult> AddDb(string id)
        {
            var vm = await _service.AddDb(id);
            return View("Details", vm);
        }

        [HttpGet]
        public async Task<IActionResult> IndexDb()
        {
            var videos = await _service.GetVideosDb();
            return View(videos);
        }
    }
}