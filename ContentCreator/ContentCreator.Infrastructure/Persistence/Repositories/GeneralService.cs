using Azure;
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

            var getRole = _roleManager.Roles.Select(x => new { x.Id, x.Name, x.RoleDescription, x.RoleType, x.IsProtected, x.AllowedFileType}).ToList();

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
                    roleResponse.IsExtensionNeeded = string.IsNullOrEmpty(role.AllowedFileType) ? false : true;

                    roleList.Add(roleResponse);
                }
                response.StatusCode = 200;
                response.Message = "success";
                response.IsSuccess = true;
                response.Result = roleList;
            }
            return response;
        }
        public async Task<ResponseData<FileTypeComboResponseModel>> GetFileTypeListAsync(string? filterFileType, CancellationToken cancellationToken)
        {
            var response = new ResponseData<FileTypeComboResponseModel>();
            response.Message = "Error in getting filetype list";

            FileTypeComboResponseModel comboResponseModel = new FileTypeComboResponseModel();

            List<FileTypeResponseModel> fileTypeList = new List<FileTypeResponseModel>();
            List<OnlyFileTypeResponseModel> onlyFileTypeList = Enum.GetValues(typeof(FileType))
                .Cast<FileType>()
                .Select(ft => new OnlyFileTypeResponseModel
                {
                    FileType = ft.ToString()
                })
                .ToList();

            var getFileType = await _context.AllowedFileTypesAndExtensions.ToListAsync(cancellationToken);

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
                extension.IsActive = request.IsActive;
                _context.AllowedFileTypesAndExtensions.Add(extension);
                await _context.SaveChangesAsync();

                response.StatusCode = 200;
                response.Message = "success";
                response.IsSuccess = true;
                response.Result = true;
            }
            return response;
        }
        public async Task<ResponseData<List<FileTypeResponseModel>>> GetExtensionListAsync(Guid RoleId, CancellationToken cancellation)
        {
            var response = new ResponseData<List<FileTypeResponseModel>> ();
            response.Message = "Error in getting filetype extension list";
            List<FileTypeResponseModel> extensionList = new List<FileTypeResponseModel>();

            var getAllowedFileType = await _roleManager.FindByIdAsync(RoleId.ToString());

            var getextensionList = await _context.AllowedFileTypesAndExtensions.Where(x=>x.FileType == getAllowedFileType.AllowedFileType).Select(x => new FileTypeResponseModel
            {
                Id = x.Id,
                FileExtension = x.FileExtension,
                FileType = x.FileType
            }).ToListAsync();
            if (getextensionList.Any())
            {
                response.StatusCode = 200;
                response.Message = "success";
                response.IsSuccess = true;
                response.Result = getextensionList;
            }

            return response;
        }
        public async Task<ResponseData<bool>> AssignExtensionsAsync(AssignExtensionsRequest request, CancellationToken cancellation)
        
        {
            var response = new ResponseData<bool>();

            var existingExtensions = await _context.AllowedExtensionOnRoles.Where(x => x.RoleId == request.RoleId && request.FileTypeId.Contains(x.FileTypeId))
                .Select(x => x.FileTypeId)
                .ToListAsync(cancellation);

            var assignExtensions = request.FileTypeId.Except(existingExtensions).ToList();

            if (!assignExtensions.Any())
            {
                response.Message = "Already Exist";
                response.StatusCode = 200;
                response.Result = false;
            }
            else
            {
                foreach(var extension in assignExtensions)
                {
                    _context.AllowedExtensionOnRoles.Add(new AllowedExtensionOnRoles
                    {
                        RoleId = request.RoleId,
                        FileTypeId = extension
                    });
                }
                await _context.SaveChangesAsync(cancellation);

                response.StatusCode = 200;
                response.Message = "Data Added Successfully";
                response.Result = true;
            }
            
            return response;          
        }
        public async Task<ResponseData<List<AssignedExtensionResponseModel>>> GetAssignedExtensionDataAsync(Guid RoleId, CancellationToken cancellation)
        {
            var response = new ResponseData<List<AssignedExtensionResponseModel>> ();
            response.Message = "Error in getting assigned file extensions";

            var getAssignedExtensionIds = await _context.AllowedExtensionOnRoles.Where(x => x.RoleId == RoleId).Select(x => x.FileTypeId).ToListAsync(cancellation);
            if (getAssignedExtensionIds.Any())
            {
                var getAssignedExtension = await _context.AllowedFileTypesAndExtensions.Where(x => getAssignedExtensionIds.Contains(x.Id)).Select(x=> new AssignedExtensionResponseModel { 
                    FileTypeId = x.Id, Extension = x.FileExtension}).ToListAsync(cancellation);
                if (getAssignedExtension.Any())
                {
                    response.StatusCode = 200;
                    response.Message = "Assigned filetype list";
                    response.Result = getAssignedExtension;
                    response.IsSuccess = true;
                }
            }
            return response;
        }
    }
}
