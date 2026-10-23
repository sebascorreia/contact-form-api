using ContactAPI.Data;
using ContactAPI.Dtos;
using ContactAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.RateLimiting;
using ContactAPI.Validation;

namespace ContactAPI.Controllers
{
    [ApiController]
    [Route("api/contact")]
    [EnableCors("ContactForm")]
    [EnableRateLimiting("ContactForm")]
    public class ContactController : ControllerBase
    {
        private readonly ContactDbContext _db;
        private readonly ILogger<ContactController> _logger;

        public ContactController(ContactDbContext db, ILogger<ContactController> logger)
        {
            _db = db;
            _logger = logger;
        }

        [HttpPost]
        [Consumes("application/json")]
        [RequestSizeLimit(32_000)]
        public async Task<IActionResult> Submit(ContactRequest request)
        {
            if (!string.IsNullOrEmpty(request.Honeypot))
            {
                _logger.LogInformation("Honeypot triggered; submission discarded");
                return Ok(new {ok=true});
            }    
            string? phone = PhoneHelper.TryNormalise(request.Phone ?? "", out var e164) ? e164 : null; 
            var submission = new ContactSubmission
            {
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                Email = request.Email.Trim().ToLowerInvariant(),
                Phone = phone,
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
