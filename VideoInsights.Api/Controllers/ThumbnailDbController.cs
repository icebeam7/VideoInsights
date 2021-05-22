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
    public class ThumbnailDbController : Controller
    {
        private readonly VideoInsightsDbContext _context;

        public ThumbnailDbController(VideoInsightsDbContext context)
        {
            _context = context;
        }

        // GET: api/<ThumbnailDbController>
        [HttpGet]
        public async Task<IActionResult> GetThumbnails()
        {
            var thumbnails = await _context.Thumbnails.ToListAsync();
            return Ok(thumbnails);
        }

        // GET api/<ThumbnailDbController>/ByExternal/5
        [HttpGet("ByExternal/{id}")]
        public async Task<IActionResult> GetThumbnailByExternal(string id)
        {
            var thumbnails = await _context.Thumbnails.Where(x => x.Externalid == id).ToListAsync();
            return (thumbnails != null) ? Ok(thumbnails) : NotFound();
        }

        // GET api/<ThumbnailDbController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetThumbnail(string id)
        {
            var thumbnail = await _context.Thumbnails.FindAsync(id);
            return (thumbnail != null) ? Ok(thumbnail) : NotFound();
        }

        // PUT api/<ThumbnailDbController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutThumbnail(string id, [FromBody] Thumbnail thumbnail)
        {
            if (id != thumbnail.Id)
                return BadRequest();

            _context.Entry(thumbnail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ThumbnailExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // POST api/<ThumbnailDbController>
        [HttpPost]
        public async Task<IActionResult> PostThumbnail([FromBody] Thumbnail thumbnail)
        {
            _context.Thumbnails.Add(thumbnail);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetThumbnail", new { id = thumbnail.Id }, thumbnail);
        }

        // DELETE api/<ThumbnailDbController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteThumbnail(string id)
        {
            var thumbnail = await _context.Thumbnails.FindAsync(id);
            if (thumbnail == null)
                return NotFound();

            _context.Thumbnails.Remove(thumbnail);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE api/<ThumbnailDbController>/All
        [HttpDelete("All")]
        public async Task<IActionResult> DeleteAll()
        {
            _context.Thumbnails.Clear();
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ThumbnailExists(string id)
        {
            return _context.Thumbnails.Any(e => e.Id == id);
        }
    }
}