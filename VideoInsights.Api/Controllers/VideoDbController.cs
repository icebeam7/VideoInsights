using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using VideoInsights.Api.Models;
using VideoInsights.Api.Helpers;
using VideoInsights.Api.Contexts;

namespace VideoInsights.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoDbController : ControllerBase
    {
        private readonly VideoInsightsDbContext _context;

        public VideoDbController(VideoInsightsDbContext context)
        {
            _context = context;
        }

        // GET: api/<VideoDbController>
        [HttpGet]
        public async Task<IActionResult> GetVideos()
        {
            var videos = await _context.Videos.OrderBy(x => x.Created).ToListAsync();
            return Ok(videos);
        }

        // GET api/<VideoDbController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVideo(string id)
        {
            var video = await _context.Videos.FindAsync(id);
            return (video != null) ? Ok(video) : NotFound();
        }

        // PUT api/<VideoDbController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVideo(string id, [FromBody] Video video)
        {
            if (id != video.Id)
                return BadRequest();

            _context.Entry(video).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VideoExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // POST api/<VideoDbController>
        [HttpPost]
        public async Task<IActionResult> PostVideo([FromBody] Video video)
        {
            _context.Videos.Add(video);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetVideo", new { id = video.Id }, video);
        }

        // DELETE api/<VideoDbController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVideo(string id)
        {
            var video = await _context.Videos.FindAsync(id);
            if (video == null)
                return NotFound();

            _context.Videos.Remove(video);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE api/<VideoDbController>/All
        [HttpDelete("All")]
        public async Task<IActionResult> DeleteAll()
        {
            _context.Videos.Clear();
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool VideoExists(string id)
        {
            return _context.Videos.Any(e => e.Id == id);
        }
    }
}