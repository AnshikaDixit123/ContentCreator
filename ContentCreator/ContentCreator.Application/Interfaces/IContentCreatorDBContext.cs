using ContentCreator.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace ContentCreator.Application.Interfaces
{
    public interface IContentCreatorDBContext
    {
        DbSet<ApplicationUser> Users { get; set; }
        DbSet<ApplicationRole> Roles { get; set; }
        DbSet<Country> Country { get; set; }
        DbSet<State> State { get; set; }
        DbSet<City> City { get; set; }
        DbSet<AllowedFileTypesAndExtensions> AllowedFileTypesAndExtensions { get; set; }
        DbSet<AllowedExtensionOnRoles> AllowedExtensionOnRoles { get; set; }
        DbSet<PostedContent> PostedContent { get; set; }
        DbSet<EmailTemplates> EmailTemplates { get; set; }
        DbSet<PostLikes> PostLikes { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
