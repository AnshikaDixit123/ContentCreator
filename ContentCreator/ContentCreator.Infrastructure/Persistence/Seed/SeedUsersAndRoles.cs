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

            //Programmer

            if (!await roleManager.RoleExistsAsync(Roles.Programmer.ToString()))
            {
                var role = new ApplicationRole { Name = Roles.Programmer.ToString(), RoleDescription = "Role for Programmer"};

                await roleManager.CreateAsync(role);
            }

            //Photographer


            if (!await roleManager.RoleExistsAsync(Roles.Photographer.ToString()))
            {
                var role = new ApplicationRole { Name = Roles.Photographer.ToString(), RoleDescription = "Role for Photographerr" };

                await roleManager.CreateAsync(role);
            }

            //Videographer


            if (!await roleManager.RoleExistsAsync(Roles.Videographer.ToString()))
            {
                var role = new ApplicationRole { Name = Roles.Videographer.ToString(), RoleDescription = "Role for Videographer" };

                await roleManager.CreateAsync(role);
            }

            //Singer


            if (!await roleManager.RoleExistsAsync(Roles.Singer.ToString()))
            {
                var role = new ApplicationRole { Name = Roles.Singer.ToString(), RoleDescription = "Role for Singer" };

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
            }

            // Create endUser if not already created

            if (await userManager.FindByNameAsync("Anshika") == null)
            {
                var endUser = new ApplicationUser
                {
                    UserName = "Anshika",
                    Email = "anshika@gmail.com",
                    PhoneNumber = "9876543211",
                    FirstName = "Anshika",
                    LastName = "Dixit",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                };

                var userPassword = "Pass@123";

                var chkUser = await userManager.CreateAsync(endUser, userPassword);

                if (chkUser.Succeeded)
                {
                    await userManager.AddToRoleAsync(endUser, Roles.User.ToString());
                }
            }

            // Create Programmer if not already created

            if (await userManager.FindByNameAsync("Arnav") == null)
            {
                var programmer = new ApplicationUser
                {
                    UserName = "Programmer",
                    Email = "programmer@gmail.com",
                    PhoneNumber = "9876543212",
                    FirstName = "Arnav",
                    LastName = "Singh",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                };

                var userPassword = "Pass@123";

                var chkUser = await userManager.CreateAsync(programmer, userPassword);

                if (chkUser.Succeeded)
                {
                    await userManager.AddToRoleAsync(programmer, Roles.Programmer.ToString());
                }
            }

            // Create Photographer if not already created

            if (await userManager.FindByNameAsync("Ayush") == null)
            {
                var photographer = new ApplicationUser
                {
                    UserName = "Photographer",
                    Email = "photographer@gmail.com",
                    PhoneNumber = "9876543213",
                    FirstName = "Ayush",
                    LastName = "Mishra",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                };

                var userPassword = "Pass@123";

                var chkUser = await userManager.CreateAsync(photographer, userPassword);

                if (chkUser.Succeeded)
                {
                    await userManager.AddToRoleAsync(photographer, Roles.Photographer.ToString());
                }
            }

            // Create Videographer if not already created

            if (await userManager.FindByNameAsync("Abhinav") == null)
            {
                var videographer = new ApplicationUser
                {
                    UserName = "Videographer",
                    Email = "abhinav@gmail.com",
                    PhoneNumber = "9876543214",
                    FirstName = "Abhinav",
                    LastName = "Dixit",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                };

                var userPassword = "Pass@123";

                var chkUser = await userManager.CreateAsync(videographer, userPassword);

                if (chkUser.Succeeded)
                {
                    await userManager.AddToRoleAsync(videographer, Roles.Videographer.ToString());
                }
            }

            // Create Singer if not already created

            if (await userManager.FindByNameAsync("Aditya") == null)
            {
                var singer = new ApplicationUser
                {
                    UserName = "Singer",
                    Email = "aditya@gmail.com",
                    PhoneNumber = "9876543215",
                    FirstName = "Aditya",
                    LastName = "Singh",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                };

                var userPassword = "Pass@123";

                var chkUser = await userManager.CreateAsync(singer, userPassword);

                if (chkUser.Succeeded)
                {
                    await userManager.AddToRoleAsync(singer, Roles.Singer.ToString());
                }
            }
        }
    }
}