using ContentCreator.Application.Common.DTOs.RequestDTOs;
using ContentCreator.Application.Common.DTOs.ResponseDTOs;
using ContentCreator.Domain.Entities.Identity;

namespace ContentCreator.Application.Interfaces
{
    public interface IGeneralService
    {
        Task<ResponseData<List<UserDetailsResponse>>> GetUserListAsync(CancellationToken cancellation);
        Task<ResponseData<List<RolesResponseModel>>> GetRoleListAsync(CancellationToken cancellation);
    }
}
