using ContentCreator.Application.Common.DTOs.ResponseDTOs;
using ContentCreator.Application.Interfaces;
using ContentCreator.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;


namespace ContentCreator.Infrastructure.Persistence.Repositories
{
    public class GeneralService : IGeneralService
    {
        private readonly IContentCreatorDBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public GeneralService(IContentCreatorDBContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<ResponseData<List<UserDetailsResponse>>> GetUserListAsync(CancellationToken cancellation)
        {
            var response = new ResponseData<List<UserDetailsResponse>>();
            response.Message = "Something went wrong";
            List<UserDetailsResponse> userList = new List<UserDetailsResponse>();

            var getUsers = _userManager.Users.Select(x => new {x.Id, x.FirstName, x.LastName, x.Email, x.PhoneNumber}).ToList();
            if (getUsers.Any())
            {
                foreach (var user in getUsers)
                {
                    var userResponse = new UserDetailsResponse();
                    userResponse.Id = user.Id;
                    userResponse.Email = user.Email;
                    userResponse.FirstName = user.FirstName;
                    userResponse.LastName = user.LastName;
                    userResponse.PhoneNumber = user.PhoneNumber;

                    userList.Add(userResponse);
                }
                response.StatusCode = 200;
                response.Message = "success";
                response.IsSuccess = true;
                response.Result = userList;
            }
            return response;
        }
    }
}
