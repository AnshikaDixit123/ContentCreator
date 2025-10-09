
using ContentCreator.Application.Common.DTOs;
using ContentCreator.Application.Common.DTOs.RequestDTOs;
using ContentCreator.Application.Common.DTOs.ResponseDTOs;
using ContentCreator.Application.Interfaces;
using ContentCreator.Domain.Entities.Identity;
using ContentCreator.Domain.Enums;
using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ContentCreator.Infrastructure.Persistence.Repositories
{
    public class AccountService : IAccountService
    {
        private readonly IContentCreatorDBContext _context;
        private readonly IDbConnection _dbConnection;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        public AccountService(IContentCreatorDBContext context, UserManager<ApplicationUser> userManager, IDbConnection dbConnection, RoleManager<ApplicationRole> roleManager, IConfiguration configuration, IEmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _dbConnection = dbConnection;
            _roleManager = roleManager;
            _configuration = configuration;
            _emailService = emailService;
        }
        public async Task<ResponseData<LoginResponseModel>> AuthenticateLoginAsync(SigningRequest request, string RoleType, CancellationToken cancellation)
        {
            var response = new ResponseData<LoginResponseModel>();
            response.Message = "Invalid username or email address";
            response.Result = new LoginResponseModel();
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
                            string tokenString = await GenerateToken(user, roleNames);
                            var loginResponse = new LoginResponseModel();
                            loginResponse.UserName = user.UserName;
                            loginResponse.UserEmail = user.Email;
                            loginResponse.UserId = user.Id;
                            loginResponse.UserRole = role.Name;
                            loginResponse.RoleType = role.RoleType;
                            loginResponse.UserPhoneNumber = user.PhoneNumber;
                            loginResponse.UserToken = tokenString;
                            response.Message = "Login sucessfully";
                            response.StatusCode = 200;
                            response.IsSuccess = true;
                            response.Result = loginResponse;
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

                string emailMsg = "We are pleased to inform you that your account creation process successfully completed on " + DateTime.Now.Date.ToString("yyyy-MM-dd") + " at " + DateTime.Now.ToString("HH:mm:ss tt") + ".";
                await _emailService.SendEmailMessageAsync(request.Email, "Account Creation", emailMsg, "0", "User");

                response.StatusCode = 200;
                response.Message = "User registered successfully";
                response.Result = true;
                response.IsSuccess = true;
            }           
            return response;
        }
        #region generate jwt token string
        private async Task<string> GenerateToken(ApplicationUser user, IList<string> roles)
        {
            var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            }.Union(roleClaims);
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddYears(1),
                signingCredentials: credentials);
            string returnToken = new JwtSecurityTokenHandler().WriteToken(token);
            return returnToken;
        } 
        #endregion
    }
}
