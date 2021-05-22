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
    public class KeyframeDbController : Controller
    {
        private readonly VideoInsightsDbContext _context;

        public KeyframeDbController(VideoInsightsDbContext context)
        {
            _context = context;
        }

        // GET: api/<KeyframeDbController>
        [HttpGet]
        public async Task<IActionResult> GetKeyframes()
        {
            var keyframes = await _context.Keyframes.ToListAsync();
            return Ok(keyframes);
        }

        // GET api/<KeyframeDbController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetKeyframe(string id)
        {
            var keyframe = await _context.Keyframes.FindAsync(id);
            return (keyframe != null) ? Ok(keyframe) : NotFound();
        }

        // GET api/<KeyframeDbController>/ByVideo/5
        [HttpGet("ByVideo/{id}")]
        public async Task<IActionResult> GetKeyframeByVideo(string id)
        {
            var keyframes = await _context.Keyframes.Where(x => x.Videoid == id).ToListAsync();
            return (keyframes != null) ? Ok(keyframes) : NotFound();
        }

        // PUT api/<KeyframeDbController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutKeyframe(string id, [FromBody] Keyframe keyframe)
        {
            if (id != keyframe.Id)
                return BadRequest();

            _context.Entry(keyframe).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!KeyframeExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // POST api/<KeyframeDbController>
        [HttpPost]
        public async Task<IActionResult> PostKeyframe([FromBody] Keyframe keyframe)
        {
            _context.Keyframes.Add(keyframe);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetKeyframe", new { id = keyframe.Id }, keyframe);
        }

        // DELETE api/<KeyframeDbController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteKeyframe(string id)
        {
            var keyframe = await _context.Keyframes.FindAsync(id);
            if (keyframe == null)
                return NotFound();

            _context.Keyframes.Remove(keyframe);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE api/<KeyframeDbController>/All
        [HttpDelete("All")]
        public async Task<IActionResult> DeleteAll()
        {
            _context.Keyframes.Clear();
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool KeyframeExists(string id)
        {
            return _context.Keyframes.Any(e => e.Id == id);
        }
    }
}