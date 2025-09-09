using ContentCreator.Application.Common.DTOs.RequestDTOs;
using ContentCreator.Application.Common.DTOs.ResponseDTOs;

namespace ContentCreator.Application.Interfaces
{
    public interface IAccountService
    {
        Task<ResponseData> AuthenticateAdminAsync(SigningRequest request, CancellationToken cancellation);
    }
}