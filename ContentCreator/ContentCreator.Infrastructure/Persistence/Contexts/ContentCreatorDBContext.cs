using ContentCreator.Application.Interfaces;
using ContentCreator.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ContentCreator.Infrastructure.Persistence.Contexts
{
    public class ContentCreatorDBContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>, IContentCreatorDBContext
    {
        public ContentCreatorDBContext(DbContextOptions<ContentCreatorDBContext> options) : base(options) { }

        public DbSet<Country> Country { get; set; }
        public DbSet<State> State { get; set; }
        public DbSet<City> City { get; set; }
        public DbSet<AllowedFileTypesAndExtensions> AllowedFileTypesAndExtensions { get; set; }
        public DbSet<AllowedExtensionOnRoles> AllowedExtensionOnRoles { get; set; }
        public DbSet<PostedContent> PostedContent { get; set; }
        public DbSet<EmailTemplates> EmailTemplates { get; set; }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => base.SaveChangesAsync(cancellationToken);

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // User Custom Config
            builder.Entity<ApplicationUser>(b =>
            {
                b.Property(u => u.FirstName).HasMaxLength(100).IsRequired();
                b.Property(u => u.MiddleName).HasMaxLength(100).IsRequired(false);
                b.Property(u => u.LastName).HasMaxLength(100).IsRequired();
                b.Property(u => u.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                b.Property(u => u.CreatedBy).HasMaxLength(100).IsRequired(false);
            });

            // Role Custom Config
            builder.Entity<ApplicationRole>(b =>
            {
                b.Property(r => r.RoleDescription).HasMaxLength(250).IsRequired(false);
            });
        }
    }
}
