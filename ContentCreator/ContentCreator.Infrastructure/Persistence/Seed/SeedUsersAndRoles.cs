using ContentCreator.Application.Interfaces;
using ContentCreator.Domain.Entities.Identity;
using ContentCreator.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ContentCreator.Infrastructure.Persistence.Seed
{
    public static class SeedUsersAndRoles
    {
        public static async Task SeedUsersAndRolesAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var _context = serviceProvider.GetRequiredService<IContentCreatorDBContext>();

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

            if (!await roleManager.RoleExistsAsync(Roles.Writer.ToString()))
            {
                var role = new ApplicationRole { Name = Roles.Writer.ToString(), RoleDescription = "Role for Programmer" };

                await roleManager.CreateAsync(role);
            }

            //Photographer


            if (!await roleManager.RoleExistsAsync(Roles.Photographer.ToString()))
            {
                var role = new ApplicationRole { Name = Roles.Photographer.ToString(), RoleDescription = "Role for Photographerr", AllowedFileType = "Image" };

                await roleManager.CreateAsync(role);
            }

            //Videographer

            if (!await roleManager.RoleExistsAsync(Roles.Videographer.ToString()))
            {
                var role = new ApplicationRole { Name = Roles.Videographer.ToString(), RoleDescription = "Role for Videographer", AllowedFileType = "Video" };

                await roleManager.CreateAsync(role);
            }

            //Singer

            if (!await roleManager.RoleExistsAsync(Roles.Singer.ToString()))
            {
                var role = new ApplicationRole { Name = Roles.Singer.ToString(), RoleDescription = "Role for Singer", AllowedFileType = "Audio" };

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

            // Create writer if not already created

            if (await userManager.FindByNameAsync("Arnav") == null)
            {
                var writer = new ApplicationUser
                {
                    UserName = "Writer",
                    Email = "writer@gmail.com",
                    PhoneNumber = "9876543212",
                    FirstName = "Arnav",
                    LastName = "Singh",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                };

                var userPassword = "Pass@123";

                var chkUser = await userManager.CreateAsync(writer, userPassword);

                if (chkUser.Succeeded)
                {
                    await userManager.AddToRoleAsync(writer, Roles.Writer.ToString());
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
            var getCountry = await _context.Country.FirstOrDefaultAsync();

            if (getCountry == null)
            {
                var country = new Country
                {
                    CountryName = "India",
                    CountryCode = "IND",
                    PhoneCode = "+91"
                };
                await _context.Country.AddAsync(country);
                await _context.SaveChangesAsync();

                var state1 = new State
                {
                    StateName = "Uttar Pradesh",
                    StateCode = "UP",
                    CountryId = country.Id
                };

                var state2 = new State
                {
                    StateName = "Uttrakhand",
                    StateCode = "UK",
                    CountryId = country.Id
                };
                await _context.State.AddRangeAsync(state1, state2);
                await _context.SaveChangesAsync();

                var cities = new List<City>
                {
                    new City { CityName = "Lucknow", StateId = state1.Id, CountryId = country.Id },
                    new City { CityName = "Noida",  StateId = state1.Id, CountryId = country.Id },
                    new City { CityName = "Dehradun", StateId = state2.Id, CountryId = country.Id },
                    new City { CityName = "Nainital", StateId = state2.Id, CountryId = country.Id }
                };

                await _context.City.AddRangeAsync(cities);
                await _context.SaveChangesAsync();
            }
            var allowedExtension = await _context.AllowedFileTypesAndExtensions.FirstOrDefaultAsync();
            if(allowedExtension == null)
            {
                var fileTypeAndExtension1 = new AllowedFileTypesAndExtensions
                {
                    FileType = "Image",
                    FileExtension = ".jpg",
                    MinimumSize = 100,
                    MaximumSize = 10000
                };
                var fileTypeAndExtension2 = new AllowedFileTypesAndExtensions
                {
                    FileType = "Video",
                    FileExtension = ".mp4",
                    MinimumSize = 500,
                    MaximumSize = 10000
                };
                var fileTypeAndExtension3 = new AllowedFileTypesAndExtensions
                {
                    FileType = "Audio",
                    FileExtension = ".mp3",
                    MinimumSize = 200,
                    MaximumSize = 5000
                };
                await _context.AllowedFileTypesAndExtensions.AddRangeAsync(fileTypeAndExtension1,fileTypeAndExtension2,fileTypeAndExtension3);
                await _context.SaveChangesAsync();


            }
            if (!await _context.EmailTemplates.AnyAsync())
            {
                _context.EmailTemplates.AddRange(new List<EmailTemplates>
                {
                    new EmailTemplates
                    {
                        Subject = "Account Creation",
                        Body = @"
                                <html xmlns='http://www.w3.org/1999/xhtml'>
                                <head><title></title></head>
                                <body>
                                <img src='https://trucastsolutions.com/wp-content/uploads/2019/09/trucast-logo.png' /><br />
                                <br />
                                <div style='border-top: 3px solid #22BCE5'>&nbsp;</div>
                                <span style='font-family: Arial; font-size: 10pt'>
                                	                Dear <b>{UserName}</b>,<br />
                                <br />
                                	                {EmailMessage}<br />
                                <br />
                                	                Thank you for choosing <b>ContentCreator</b>.<br />
                                <br />
                                	                Best regards,<br />
                                <b>Team ContentCreator</b>
                                </span>
                                </body>
                                </html>"
                    },
                    new EmailTemplates
                    {
                        Subject = "Password Reset OTP for Your ContentCreator Account",
                        Body = @"
                                <html xmlns='http://www.w3.org/1999/xhtml'>
                                <head><title></title></head>
                                <body>
                                <img src='https://trucastsolutions.com/wp-content/uploads/2019/09/trucast-logo.png' /><br />
                                <br />
                                <div style='border-top: 3px solid #22BCE5'>&nbsp;</div>
                                <span style='font-family: Arial; font-size: 10pt'>
                                	                Dear <b>{UserName}</b>,<br />
                                <br />
                                	                {EmailMessage}<br />
                                <br />
                                <b>{OTP}</b><br />
                                <br />
                                	                This OTP is valid for the next 15 minutes. If you did not request a password reset, please ignore this email or contact our support team immediately.<br />
                                <br />
                                	                Thank you for choosing <b>ContentCreator</b>.<br />
                                <br />
                                	                Best regards,<br />
                                <b>Team ContentCreator</b>
                                </span>
                                </body>
                                </html>"
                    }
                });
                await _context.SaveChangesAsync();
            }

        }
    }
}