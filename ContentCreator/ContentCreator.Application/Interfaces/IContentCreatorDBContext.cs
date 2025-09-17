using ContentCreator.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace ContentCreator.Application.Interfaces
{
    public interface IContentCreatorDBContext
    {
        DbSet<ApplicationUser> Users { get; }
        DbSet<ApplicationRole> Roles { get; }
        DbSet<Country> Country { get; }
        DbSet<State> State { get; }
        DbSet<City> City { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
