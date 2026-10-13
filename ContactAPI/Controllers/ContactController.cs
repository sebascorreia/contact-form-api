using ContactAPI.Data;
using ContactAPI.Dtos;
using ContactAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.RateLimiting;

namespace ContactAPI.Controllers
{
    [ApiController]
    [Route("api/contact")]
    [EnableCors("ContactForm")]
    [EnableRateLimiting("ContactForm")]
    public class ContactController : ControllerBase
    {
        private readonly ContactDbContext _db;

        public ContactController(ContactDbContext db)
        {
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> Submit(ContactRequest request)
        {
            var submission = new ContactSubmission
            {
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                Email = request.Email.Trim().ToLowerInvariant(),
                Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim(),
                Country = string.IsNullOrWhiteSpace(request.Country) ? null : request.Country.Trim(),
                Message = request.Message.Trim(),
                Roles = request.Roles,
                Source = request.Source,
            };
            _db.ContactSubmissions.Add(submission);
            await _db.SaveChangesAsync();

            return Ok(new { ok = true });
        }
    }
}
