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
        private readonly RoleManager<ApplicationRole> _roleManager;

        public GeneralService(IContentCreatorDBContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<ResponseData<List<UserDetailsResponse>>> GetUserListAsync(CancellationToken cancellation)
        {
            var response = new ResponseData<List<UserDetailsResponse>>();
            response.Message = "Something went wrong";
            List<UserDetailsResponse> userList = new List<UserDetailsResponse>();

            var getUsers = _userManager.Users.ToList();
            if (getUsers.Any())
            {
                foreach (var user in getUsers)
                {
                    var userResponse = new UserDetailsResponse();
                    userResponse.Id = user.Id;
                    userResponse.Email = user.Email;
                    userResponse.FullName = user.FirstName + " " + user.LastName;
                    userResponse.PhoneNumber = user.PhoneNumber;
                    var roles = await _userManager.GetRolesAsync(user);
                    userResponse.Role = roles.FirstOrDefault() ?? "";

                    userList.Add(userResponse);
                }
                response.StatusCode = 200;
                response.Message = "success";
                response.IsSuccess = true;
                response.Result = userList;
            }
            return response;
        }
        public async Task<ResponseData<List<RolesResponseModel>>> GetRoleListAsync(CancellationToken cancellation)
        {
            var response = new ResponseData<List<RolesResponseModel>>();
            response.Message = "Something went wrong";
            List<RolesResponseModel> roleList = new List<RolesResponseModel>();

            var getRole = _roleManager.Roles.Select(x => new { x.Id, x.Name, x.RoleDescription, x.RoleType, x.IsProtected }).ToList();
            if (getRole.Any())
            {
                foreach (var role in getRole)
                {
                    var roleResponse = new RolesResponseModel();
                    
                    roleResponse.Id = role.Id;
                    roleResponse.RoleName = role.Name;
                    roleResponse.RoleDescription = role.RoleDescription;
                    roleResponse.RoleType = role.RoleType;
                    roleResponse.IsProtected = role.IsProtected;
                    var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
                    roleResponse.UserCount = usersInRole.Count;

                    roleList.Add(roleResponse);
                }
                response.StatusCode = 200;
                response.Message = "success";
                response.IsSuccess = true;
                response.Result = roleList;
            }
            return response;
        }
    }
}
