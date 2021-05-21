using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
//using Microsoft.EntityFrameworkCore;

using VideoInsights.Models;
//using VideoInsights.Api.Context;

namespace VideoInsights.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoDbController : ControllerBase
    {
        /*private readonly VideoDbContext _context;

        public VideoDbController(VideoDbContext context)
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
        public async Task<IActionResult> GetVideo(int id)
        {
            var video = await _context.Videos.FindAsync(id);
            return (video != null) ? Ok(video) : NotFound();
        }

        // PUT api/<VideoDbController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVideo(int id, [FromBody] Video video)
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
            _context.Video.Add(video);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetVideo", new { id = video.Id }, video);
        }

        // DELETE api/<VideoDbController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVideo(int id)
        {
            var video = await _context.Video.FindAsync(id);
            if (video == null)
                return NotFound();

            _context.Video.Remove(video);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool VideoExists(int id)
        {
            return _context.Video.Any(e => e.Id == id);
        }
        */
    }
}