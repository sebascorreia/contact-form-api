using System.ComponentModel.DataAnnotations;
namespace ContactAPI.Models
{
    public class ContactSubmission
    {
        public int Id { get; set; }

        [MaxLength(60)]
        public required string FirstName { get; set; }

        [MaxLength (60)]
        public required string LastName { get; set; }

        [MaxLength (254)]
        public required string Email { get; set; }

        [MaxLength (20)]
        public string? Phone { get; set; }

        [MaxLength (100)]
        public string? Country { get; set; }

        [MaxLength (5000)]
        public required string Message {  get; set; }

        public List<string> Roles { get; set; } = [];

        [MaxLength (50)]
        public required string Source { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
