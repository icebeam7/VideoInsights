using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using VideoInsights.Api.Services;

namespace VideoInsights.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoIndexerController : ControllerBase
    {
        readonly IVideoIndexerService _videoIndexerService;

        public VideoIndexerController(IVideoIndexerService videoIndexerService)
        {
            _videoIndexerService = videoIndexerService;
        }

        // GET: api/<VideoController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var videos = await _videoIndexerService.GetVideos();
            return (videos != null) ? Ok(videos) : NotFound();
        }

        // GET: api/<VideoController>/id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetails(string id)
        {
            var details = await _videoIndexerService.GetVideoDetails(id);
            return (details != null) ? Ok(details) : NotFound();
        }

        // POST api/<VideoController>
        [HttpPost]
        public async Task<ActionResult> Post(IFormFile file)
        {
            var op = await _videoIndexerService.UploadVideo(file);
            return (op) ? Ok() : UnprocessableEntity();
        }
    }
}