using ContentCreator.Application.Interfaces;
using ContentCreator.Domain.Entities.Identity;
using ContentCreator.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace ContentCreator.Infrastructure.Persistence.Seed
{
    public static class SeedUsersAndRoles
    {
        public static async Task SeedUsersAndRolesAsync(IServiceProvider serviceProvider)
        {
            try
            {
                var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
                var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                //var dbContext = serviceProvider.GetRequiredService<IContentCreatorDBContext>();

                // Super Admin

                if (!await roleManager.RoleExistsAsync(Roles.SuperAdmin.ToString()))
                {
                    var role = new ApplicationRole { Name = Roles.SuperAdmin.ToString(), RoleDescription = "Role for super admin user" };

                    await roleManager.CreateAsync(role);
                }

                // User

                if (!await roleManager.RoleExistsAsync(Roles.User.ToString()))
                {
                    var role = new ApplicationRole { Name = Roles.User.ToString(), RoleDescription = "Role for end user" };

                    await roleManager.CreateAsync(role);
                }

                // Create Super Admin if not already created

                if (await userManager.FindByNameAsync("SuperAdmin") == null)
                {
                    var adminUser = new ApplicationUser
                    {
                        UserName = "SuperAdmin",
                        Email = "superadmin@gmail.com",
                        PhoneNumber = "9876543210",
                        FirstName = "Super",
                        LastName = "Admin",
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true,
                    };

                    var userPassword = "Pass@123";

                    var chkUser = await userManager.CreateAsync(adminUser, userPassword);

                    if (chkUser.Succeeded)
                    {
                        await userManager.AddToRoleAsync(adminUser, Roles.SuperAdmin.ToString());
                    }
                    else
                    {

                    }
                }
            }
            catch(Exception ex)
            {

            }
        }
    }
}