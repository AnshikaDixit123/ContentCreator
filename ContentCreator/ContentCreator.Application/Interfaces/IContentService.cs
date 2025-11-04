using ContentCreator.Application.Common.DTOs.RequestDTOs;
using ContentCreator.Application.Common.DTOs.ResponseDTOs;


namespace ContentCreator.Application.Interfaces
{
    public interface IContentService
    {
        Task<ResponseData<bool>> UploadAPostAsync(UploadAPostRequest request, CancellationToken cancellation);
        Task<ResponseData<List<AllowedExtensionToCreatorResponseModel>>> GetAllowedExtensionToCreatorAsync(Guid RoleId, CancellationToken cancellation);
        Task<ResponseData<List<GetPostResponseModel>>> GetPostAsync(Guid userId, CancellationToken cancellation);
        Task<ResponseData<bool>> PostLikesAsync(PostLikesRequestModel request, CancellationToken cancellation);
        Task<ResponseData<bool>> PostCommentsAsync(PostCommentsRequestModel request, CancellationToken cancellation);
        Task<ResponseData<List<GetCommentsResponseModel>>> GetCommentsAsync(Guid postId, CancellationToken cancellation);
        Task<ResponseData<List<GetCommentsResponseModel>>> GetRepliesAsync(Guid commentId, CancellationToken cancellation);
    }
}
