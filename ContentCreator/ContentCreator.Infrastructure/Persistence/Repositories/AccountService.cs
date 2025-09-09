using ContentCreator.Application.Common.DTOs.RequestDTOs;
using ContentCreator.Application.Common.DTOs.ResponseDTOs;
using ContentCreator.Application.Interfaces;
using ContentCreator.Domain.Entities.Identity;
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
        public async Task<ResponseData> AuthenticateAdminAsync(SigningRequest request, CancellationToken cancellation)
        {
            var response = new ResponseData();
            response.Message = "Invalid username or email address";
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
                }
            }
            return response;
        }
    }
}
