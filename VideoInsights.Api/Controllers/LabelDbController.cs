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
    public class LabelDbController : Controller
    {
        private readonly VideoInsightsDbContext _context;

        public LabelDbController(VideoInsightsDbContext context)
        {
            _context = context;
        }

        // GET: api/<LabelDbController>
        [HttpGet]
        public async Task<IActionResult> GetLabels()
        {
            var labels = await _context.Labels.ToListAsync();
            return Ok(labels);
        }

        // GET api/<LabelDbController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLabel(string id)
        {
            var label = await _context.Labels.FindAsync(id);
            return (label != null) ? Ok(label) : NotFound();
        }

        // GET api/<LabelDbController>/ByKeyframe/5
        [HttpGet("ByKeyframe/{id}")]
        public async Task<IActionResult> GetLabelByKeyframe(string id)
        {
            var labels = await _context.Labels.Where(x => x.Keyframeid == id).ToListAsync();
            return (labels != null) ? Ok(labels) : NotFound();
        }

        // PUT api/<LabelDbController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLabel(string id, [FromBody] Label label)
        {
            if (id != label.Id)
                return BadRequest();

            _context.Entry(label).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LabelExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // POST api/<LabelDbController>
        [HttpPost]
        public async Task<IActionResult> PostLabel([FromBody] Label label)
        {
            _context.Labels.Add(label);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLabel", new { id = label.Id }, label);
        }

        // DELETE api/<LabelDbController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLabel(string id)
        {
            var label = await _context.Labels.FindAsync(id);
            if (label == null)
                return NotFound();

            _context.Labels.Remove(label);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE api/<LabelDbController>/All
        [HttpDelete("All")]
        public async Task<IActionResult> DeleteAll()
        {
            _context.Labels.Clear();
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LabelExists(string id)
        {
            return _context.Labels.Any(e => e.Id == id);
        }
    }
}