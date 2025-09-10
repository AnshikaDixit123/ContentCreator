using ContentCreator.Application.Common.DTOs.RequestDTOs;
using ContentCreator.Application.Common.DTOs.ResponseDTOs;
using ContentCreator.Application.Interfaces;
using ContentCreator.Domain.Entities.Identity;
using ContentCreator.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace ContentCreator.Infrastructure.Persistence.Repositories
{
    public class AccountService : IAccountService
    {
        private readonly IContentCreatorDBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public AccountService(IContentCreatorDBContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<ResponseData<bool>> AuthenticateAdminAsync(SigningRequest request, CancellationToken cancellation)
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
            return response;
        }
        public async Task<ResponseData<bool>> SignUpAsync(SignUpRequest request, CancellationToken cancellation)
        {
            var response = new ResponseData<bool>();
            response.Message = "Invalid username or email address or phone no.";
            response.Result = false;

            var existingByEmail = await _userManager.FindByEmailAsync(request.Email);
            if (existingByEmail != null)
            {
                response.Message = "Email already exists.";
                return response;
            }

            var existingByUserName = await _userManager.FindByNameAsync(request.UserName);
            if (existingByUserName != null)
            {
                response.Message = "Username already exists.";
                return response;
            }

            var existingByPhone = _userManager.Users.FirstOrDefault(u => u.PhoneNumber == request.PhoneNumber);
            if (existingByPhone != null)
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
            }           
            return response;
        }
    }
}
