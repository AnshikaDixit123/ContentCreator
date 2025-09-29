using ContentCreator.Application.Common.DTOs.RequestDTOs;
using ContentCreator.Application.Common.DTOs.ResponseDTOs;
using ContentCreator.Application.Interfaces;
using ContentCreator.Domain.Entities.Identity;
using ContentCreator.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


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
        public async Task<ResponseData<List<RolesResponseModel>>> GetRoleListAsync(bool? IncludeSuperAdmin, CancellationToken cancellation)
        {
            bool includeSuperAdmin = IncludeSuperAdmin ?? false;
            var response = new ResponseData<List<RolesResponseModel>>();
            response.Message = "Something went wrong";
            List<RolesResponseModel> roleList = new List<RolesResponseModel>();

            var getRole = _roleManager.Roles.Select(x => new { x.Id, x.Name, x.RoleDescription, x.RoleType, x.IsProtected }).ToList();
            if (getRole.Any())
            {
                foreach (var role in getRole)
                {
                    if (!includeSuperAdmin && role.Name == Roles.SuperAdmin.ToString())
                        continue;

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
        public async Task<ResponseData<FileTypeComboResponseModel>> GetFileTypeListAsync(CancellationToken cancellationToken)
        {
            string filterFileType = string.Empty;
            if (false)
                filterFileType = "Image";
            var response = new ResponseData<FileTypeComboResponseModel>();
            response.Message = "Error in getting filetype list";

            FileTypeComboResponseModel comboResponseModel = new FileTypeComboResponseModel();

            List<FileTypeResponseModel> fileTypeList = new List<FileTypeResponseModel>();
            List<OnlyFileTypeResponseModel> onlyFileTypeList = new List<OnlyFileTypeResponseModel>();

            var getFileType = await _context.AllowedFileTypesAndExtensions.ToListAsync();

            var fileTypeOnly = getFileType.GroupBy(x => x.FileType).Select(x=>x.Key).ToList();
            foreach(var item in fileTypeOnly)
            {
                var fileType = new OnlyFileTypeResponseModel();
                fileType.FileType = item;
                onlyFileTypeList.Add(fileType);
            }

            if (!string.IsNullOrEmpty(filterFileType))
                getFileType = getFileType.Where(x => x.FileType == filterFileType).ToList();

            foreach ( var fileType in getFileType)
            {
                var fileTypeResponse = new FileTypeResponseModel();
                fileTypeResponse.Id = fileType.Id;
                fileTypeResponse.FileType = fileType.FileType;
                fileTypeResponse.FileExtension = fileType.FileExtension;
                fileTypeResponse.MinimumSize = fileType.MinimumSize;
                fileTypeResponse.MaximumSize = fileType.MaximumSize;
                fileTypeResponse.IsActive = fileType.IsActive;
                fileTypeList.Add(fileTypeResponse);
            }

            comboResponseModel.FileTypeList = onlyFileTypeList;
            comboResponseModel.FileTypeDetailList = fileTypeList;

            response.StatusCode = 200;
            response.Message = "success";
            response.IsSuccess = true;
            response.Result = comboResponseModel;

            return response;
        }
        public async Task<ResponseData<bool>> AddExtensionAsync(AddExtensionsRequest request, CancellationToken cancellation)
        {
            var response = new ResponseData<bool>();
            response.Message = "No new extension added";
            var newExtension = await _context.AllowedFileTypesAndExtensions.Where(x => 
            x.FileType == request.FileType && 
            x.FileExtension == request.FileExtension).
            FirstOrDefaultAsync(cancellation);
            if (newExtension != null)
            {
                if(newExtension.FileType == request.FileType && newExtension.FileExtension == request.FileExtension)
                {
                    response.Message = "FileType with this extension already exist";
                }
            }
            else
            {
                var extension = new AllowedFileTypesAndExtensions();
                extension.FileType = request.FileType;
                extension.FileExtension = request.FileExtension;
                extension.MinimumSize = request.MinimumSize;
                extension.MaximumSize = request.MaximumSize;
                _context.AllowedFileTypesAndExtensions.Add(extension);
                await _context.SaveChangesAsync();

                response.StatusCode = 200;
                response.Message = "success";
                response.IsSuccess = true;
                response.Result = true;
            }
            return response;
        }
    }
}
