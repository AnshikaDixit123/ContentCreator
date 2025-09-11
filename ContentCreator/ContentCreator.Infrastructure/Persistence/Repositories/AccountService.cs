using ContentCreator.Application.Common.DTOs;
using ContentCreator.Application.Common.DTOs.RequestDTOs;
using ContentCreator.Application.Common.DTOs.ResponseDTOs;
using ContentCreator.Application.Interfaces;
using ContentCreator.Domain.Entities.Identity;
using ContentCreator.Domain.Enums;
using Dapper;
using Microsoft.AspNetCore.Identity;
using System.Data;

namespace ContentCreator.Infrastructure.Persistence.Repositories
{
    public class AccountService : IAccountService
    {
        private readonly IContentCreatorDBContext _context;
        private readonly IDbConnection _dbConnection;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        public AccountService(IContentCreatorDBContext context, UserManager<ApplicationUser> userManager, IDbConnection dbConnection, RoleManager<ApplicationRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _dbConnection = dbConnection;
            _roleManager = roleManager;
        }
        public async Task<ResponseData<bool>> AuthenticateLoginAsync(SigningRequest request, string RoleType, CancellationToken cancellation)
        {
            var response = new ResponseData<bool>();
            response.Message = "Invalid username or email address";
            response.Result = false;
            var user = new ApplicationUser();

            user = await _userManager.FindByNameAsync(request.UserNameOrEmail);
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(request.UserNameOrEmail);
            }

            if (user != null)
            {
                var roleNames = await _userManager.GetRolesAsync(user); // returns role names like "SuperAdmin", "User"
                var roleName = roleNames.FirstOrDefault(); // since user has only one role
                response.Message = "User role not defined";
                if (roleName != null)
                {
                    var role = await _roleManager.FindByNameAsync(roleName);
                    if(role.RoleType == RoleType)
                    {
                        response.Message = "Invalid password";
                        var result = await _userManager.CheckPasswordAsync(user, request.Password);
                        if (result)
                        {
                            response.Message = "Login sucessfully";
                            response.StatusCode = 200;
                            response.IsSuccess = true;
                            response.Result = true;
                        }
                    }
                }
            }
            return response;
        }
        public async Task<ResponseData<bool>> SignUpAsync(SignUpRequest request, CancellationToken cancellation)
        {
            var response = new ResponseData<bool>
            {
                Message = "Invalid username or email address or phone no.",
                Result = false
            };

            // Normalize to match Identity’s internal storage
            var normalizedEmail = _userManager.NormalizeEmail(request.Email);
            var normalizedUserName = _userManager.NormalizeName(request.UserName);

            string sql = @"
                SELECT 
                    CASE WHEN EXISTS (SELECT 1 FROM AspNetUsers WHERE NormalizedEmail = @Email) THEN 1 ELSE 0 END AS EmailExists,
                    CASE WHEN EXISTS (SELECT 1 FROM AspNetUsers WHERE NormalizedUserName = @UserName) THEN 1 ELSE 0 END AS UserNameExists,
                    CASE WHEN EXISTS (SELECT 1 FROM AspNetUsers WHERE PhoneNumber = @PhoneNumber) THEN 1 ELSE 0 END AS PhoneExists;";

            var exists = await _dbConnection.QuerySingleAsync<UserExistsDto>(sql, new
            {
                Email = normalizedEmail,
                UserName = normalizedUserName,
                PhoneNumber = request.PhoneNumber
            });

            if (exists.EmailExists)
            {
                response.Message = "Email already exists.";
                return response;
            }
            if (exists.UserNameExists)
            {
                response.Message = "Username already exists.";
                return response;
            }
            if (exists.PhoneExists)
            {
                response.Message = "Phone number already exists.";
                return response;
            }

            var user = new ApplicationUser
            {
                UserName = request.UserName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                FirstName = request.FirstName,
                LastName = request.LastName,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };
            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, Roles.User.ToString()); // assign default role
                response.StatusCode = 201;
                response.Message = "User registered successfully";
                response.Result = true;
                response.IsSuccess = true;
            }           
            return response;
        }
    }
}
