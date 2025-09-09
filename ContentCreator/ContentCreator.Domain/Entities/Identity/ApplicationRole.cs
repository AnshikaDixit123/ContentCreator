using Microsoft.AspNetCore.Identity;

namespace ContentCreator.Domain.Entities.Identity
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        public string? RoleDescription { get; set; }  // optional description for the role
    }
    
}
