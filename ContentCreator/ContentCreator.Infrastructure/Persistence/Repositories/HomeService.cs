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
    public class HomeService : IHomeService
    {
        private readonly IContentCreatorDBContext _context;
        private readonly IDbConnection _dbConnection;
        private readonly UserManager<ApplicationUser> _userManager;
        public HomeService(IContentCreatorDBContext context, UserManager<ApplicationUser> userManager, IDbConnection dbConnection)
        {
            _context = context;
            _userManager = userManager;
            _dbConnection = dbConnection;
        }
        public async Task<ResponseData<bool>> CreateUserAsync(CreateNewUserRequest request, CancellationToken cancellation)
        {
            var response = new ResponseData<bool>
            {
                Message = "Invalid username or email address or phone no.",
                Result = false
            };

            // Normalize to match Identity’s internal storage
            var normalizedEmail = _userManager.NormalizeEmail(request.EmailAddress);
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
                Email = request.EmailAddress,
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
                response.StatusCode = 200;
                response.Message = "User registered successfully";
                response.Result = true;
                response.IsSuccess = true;
            }

            return response;
        }
    }
}
