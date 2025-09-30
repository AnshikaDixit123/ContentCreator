using Microsoft.AspNetCore.Identity;

namespace ContentCreator.Domain.Entities.Identity
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        public string? RoleDescription { get; set; }  // optional description for the role
        public string? RoleType { get; set; }  // nvarchar(50), nullable
        public bool IsProtected { get; set; } = false; // bit, default 0
        public string? AllowedFileType { get; set; }
    }
    
}
