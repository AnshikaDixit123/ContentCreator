using ContentCreator.Application.Common.DTOs.RequestDTOs;
using ContentCreator.Application.Common.DTOs.ResponseDTOs;
using ContentCreator.Domain.Entities.Identity;

namespace ContentCreator.Application.Interfaces
{
    public interface IGeneralService
    {
        Task<ResponseData<List<UserDetailsResponse>>> GetUserListAsync(CancellationToken cancellation);
        Task<ResponseData<FileTypeComboResponseModel>> GetFileTypeListAsync(string? filterFileType, CancellationToken cancellation);
        Task<ResponseData<List<RolesResponseModel>>> GetRoleListAsync(bool? IncludeSuperAdmin, CancellationToken cancellation);
        Task<ResponseData<bool>> AddExtensionAsync(AddExtensionsRequest request, CancellationToken cancellation);
    }
}
