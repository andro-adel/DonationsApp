using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DonationsApp.Data;
using DonationsApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace DonationsApp.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DonationsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DonationsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Donations (Read All)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Donation>>> GetDonations()
        {
            // LINQ Query: Deferred Execution
            var query = _context.Donations
                .Where(d => d.Amount > 0)  // Filter
                .OrderByDescending(d => d.DonationDate)  // Sort
                .Select(d => new { d.Id, d.Amount, d.DonorName });  // Projection

            // ToListAsync() تنفيذ الـ Query (Eager Execution)
            var results = await query.ToListAsync();

            // شرح Performance: Where قبل Select عشان يقلل البيانات المقروءة (Big-O: O(n) للـ Scan)
            return Ok(results);
        }

        // GET: api/Donations/5 (Read Single)
        [HttpGet("{id}")]
        public async Task<ActionResult<Donation>> GetDonation(int id)
        {
            // AsNoTracking() لتحسين الأداء (No Change Tracking)
            var donation = await _context.Donations
                .AsNoTracking()  // Advanced: Disable Tracking for Read-Only
                .FirstOrDefaultAsync(d => d.Id == id);

            if (donation == null) return NotFound();

            return donation;
        }

        // POST: api/Donations (Create)
        [HttpPost]
        public async Task<ActionResult<Donation>> PostDonation(Donation donation)
        {
            _context.Donations.Add(donation);
            await _context.SaveChangesAsync();  // Commit

            return CreatedAtAction(nameof(GetDonation), new { id = donation.Id }, donation);
        }

        // PUT: api/Donations/5 (Update)
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDonation(int id, Donation donation)
        {
            if (id != donation.Id) return BadRequest();

            _context.Entry(donation).State = EntityState.Modified;  // Tracking Advanced
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DonationExists(id)) return NotFound();
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Donations/5 (Delete)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDonation(int id)
        {
            var donation = await _context.Donations.FindAsync(id);
            if (donation == null) return NotFound();

            _context.Donations.Remove(donation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DonationExists(int id)
        {
            return _context.Donations.Any(e => e.Id == id);  // LINQ Any() للـ Check (Efficient O(1) مع Index)
        }
    }
}
