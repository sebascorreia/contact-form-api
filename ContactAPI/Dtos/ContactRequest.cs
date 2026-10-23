using System.ComponentModel.DataAnnotations;
using ContactAPI.Validation;
using Microsoft.Extensions.Options;
namespace ContactAPI.Dtos
{
    public class ContactRequest : IValidatableObject
    {
        private const string NamePattern = @"^[\p{L}\p{M}\s''.\-]+$";

        private static readonly HashSet<string> AllowedRoles =
            [
                "Artist", "Prospective Client", "Curator", "Gallery",
                "Art Advisor", "Institution", "Press", "Other",
            ];
        private static readonly HashSet<string> AllowedSources =
            ["afa-contact-form", "website-contact"];

        [Required, StringLength(60), RegularExpression(NamePattern)]
        public string FirstName { get; set; } = string.Empty;

        [Required, StringLength(60), RegularExpression(NamePattern)]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(254)]
        public string Email { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(100), RegularExpression(NamePattern)]
        public string? Country { get; set; }

        [Required, StringLength(5000)]
        public string Message { get; set; } = string.Empty;

        [MaxLength(8)]
        public List<string> Roles { get; set; } = [];

        [StringLength(50)]
        public string Source { get; set; } = "afa-contact-form";

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!AllowedSources.Contains(Source))
                yield return new ValidationResult("Invalid source", [nameof(Source)]);
            if (Roles.Any(role => !AllowedRoles.Contains(role)))
                yield return new ValidationResult("Invalid role", [nameof (Roles)]);
            if (Source != "website-contact" && Roles.Count == 0)
                yield return new ValidationResult("At least one role is required", [nameof(Roles)]);
            if (!string.IsNullOrWhiteSpace(Phone) && !PhoneHelper.TryNormalise(Phone, out _))
                yield return new ValidationResult("Invalid phone number", [nameof(Phone)]);
        }
    }
}
