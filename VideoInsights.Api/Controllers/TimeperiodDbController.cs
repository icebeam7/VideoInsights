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
    public class TimeperiodDbController : Controller
    {
        private readonly VideoInsightsDbContext _context;

        public TimeperiodDbController(VideoInsightsDbContext context)
        {
            _context = context;
        }

        // GET: api/<TimeperiodDbController>
        [HttpGet]
        public async Task<IActionResult> GetTimeperiods()
        {
            var timeperiods = await _context.Timeperiods.ToListAsync();
            return Ok(timeperiods);
        }

        // GET api/<TimeperiodDbController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTimeperiod(string id)
        {
            var timeperiod = await _context.Timeperiods.FindAsync(id);
            return (timeperiod != null) ? Ok(timeperiod) : NotFound();
        }

        // GET api/<TimeperiodDbController>/ByKeyframe/5
        [HttpGet("ByKeyframe/{id}")]
        public async Task<IActionResult> GetTimePeriodByKeyframe(string id)
        {
            var timeperiods = await _context.Timeperiods.Where(x => x.Keyframeid == id).ToListAsync();
            return (timeperiods != null) ? Ok(timeperiods) : NotFound();
        }

        // PUT api/<TimeperiodDbController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTimeperiod(string id, [FromBody] Timeperiod timeperiod)
        {
            if (id != timeperiod.Id)
                return BadRequest();

            _context.Entry(timeperiod).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TimeperiodExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // POST api/<TimeperiodDbController>
        [HttpPost]
        public async Task<IActionResult> PostTimeperiod([FromBody] Timeperiod timeperiod)
        {
            _context.Timeperiods.Add(timeperiod);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTimeperiod", new { id = timeperiod.Id }, timeperiod);
        }

        // DELETE api/<TimeperiodDbController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTimeperiod(string id)
        {
            var timeperiod = await _context.Timeperiods.FindAsync(id);
            if (timeperiod == null)
                return NotFound();

            _context.Timeperiods.Remove(timeperiod);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE api/<TimeperiodDbController>/All
        [HttpDelete("All")]
        public async Task<IActionResult> DeleteAll()
        {
            _context.Timeperiods.Clear();
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TimeperiodExists(string id)
        {
            return _context.Timeperiods.Any(e => e.Id == id);
        }
    }
}