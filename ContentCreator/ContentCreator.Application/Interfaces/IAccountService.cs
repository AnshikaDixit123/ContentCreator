using ContentCreator.Application.Common.DTOs.RequestDTOs;
using ContentCreator.Application.Common.DTOs.ResponseDTOs;

namespace ContentCreator.Application.Interfaces
{
    public interface IAccountService
    {
        Task<ResponseData<bool>> AuthenticateAdminAsync(SigningRequest request, CancellationToken cancellation);
        Task<ResponseData<bool>> SignUpAsync(SignUpRequest request, CancellationToken cancellation);
    }
}