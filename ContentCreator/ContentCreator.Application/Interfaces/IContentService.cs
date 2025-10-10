using ContentCreator.Application.Common.DTOs.RequestDTOs;
using ContentCreator.Application.Common.DTOs.ResponseDTOs;


namespace ContentCreator.Application.Interfaces
{
    public interface IContentService
    {
        Task<ResponseData<bool>> UploadAPostAsync(UploadAPostRequest request, CancellationToken cancellation);
        Task<ResponseData<List<AllowedExtensionToCreatorResponseModel>>> GetAllowedExtensionToCreatorAsync(Guid RoleId, CancellationToken cancellation);
    }
}
