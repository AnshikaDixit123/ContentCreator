using Microsoft.AspNetCore.Identity;

namespace ContentCreator.Domain.Entities.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        // Personal name fields
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }    // nullable
        public string LastName { get; set; } = string.Empty;

        // Auditing info
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }     // nullable (who created the user)
    }
}
