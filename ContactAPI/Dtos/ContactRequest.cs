using System.ComponentModel.DataAnnotations;

namespace ContactAPI.Dtos
{
    public class ContactRequest
    {
        [Required, StringLength(60)]
        public string FirstName { get; set; } = string.Empty;

        [Required, StringLength(60)]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(254)]
        public string Email { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Phone {  get; set; }

        [StringLength(100)]
        public string? Country {  get; set; }

        [Required, StringLength(5000)]
        public string Message {  get; set; } = string.Empty;

        [MaxLength(8)]
        public List<string> Roles { get; set; } = [];

        [StringLength(50)]
        public string Source {  get; set; } = "contact-form";
    }
}
