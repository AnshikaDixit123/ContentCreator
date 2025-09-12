using ContentCreator.Application.Common.DTOs.RequestDTOs;
using ContentCreator.Application.Common.DTOs.ResponseDTOs;

namespace ContentCreator.Application.Interfaces
{
    public interface IAccountService
    {
        Task<ResponseData<LoginResponseModel>> AuthenticateLoginAsync(SigningRequest request, string RoleType, CancellationToken cancellation);
        Task<ResponseData<bool>> SignUpAsync(SignUpRequest request, CancellationToken cancellation);
    }
}