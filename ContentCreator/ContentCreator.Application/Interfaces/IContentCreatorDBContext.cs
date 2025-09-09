using ContentCreator.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace ContentCreator.Application.Interfaces
{
    public interface IContentCreatorDBContext
    {
        DbSet<ApplicationUser> Users { get; }
        DbSet<ApplicationRole> Roles { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
