using ContentCreator.Application.Common.DTOs.RequestDTOs;
using ContentCreator.Application.Common.DTOs.ResponseDTOs;

namespace ContentCreator.Application.Interfaces
{
    public interface IHomeService
    {
        Task<ResponseData<bool>> CreateUserAsync(CreateNewUserRequest request, CancellationToken cancellation);
        Task<ResponseData<bool>> GetMyProfileAsync(GetMyProfileRequest request, CancellationToken cancellation);
        Task<ResponseData<bool>> CreateRolesAsync(CreateRolesRequest request, CancellationToken cancellation);
    }
}
