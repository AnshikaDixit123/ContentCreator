using Azure.Core;
using ContentCreator.Application.Common.DTOs.RequestDTOs;
using ContentCreator.Application.Common.DTOs.ResponseDTOs;
using ContentCreator.Application.Interfaces;
using ContentCreator.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ContentCreator.Infrastructure.Persistence.Repositories
{
    public class ContentService : IContentService
    {
        private readonly IContentCreatorDBContext _context;
        public ContentService(IContentCreatorDBContext context)
        {
            _context = context;
        }
        public async Task<ResponseData<bool>> UploadAPostAsync(UploadAPostRequest request, CancellationToken cancellation)
        {
            var response = new ResponseData<bool>();
            response.Message = "Post is not uploaded";

            var postEntity = new PostedContent
            {
                UserId = request.UserId,
                PostDescription = request.PostDescription,
                MediaUrl = request.FilePath,
                IsPublic = request.Visibility == "public",
                IsPrivate = request.Visibility == "private",
                IsSubscribed = request.Visibility == "subscribed"
                //IsPublic = request.IsPublic,
                //IsPrivate = request.IsPrivate,
                //IsSubscribed = request.IsSubscribed
            };
            _context.PostedContent.Add(postEntity);
            await _context.SaveChangesAsync(cancellation);

            response.StatusCode = 200;
            response.Message = "success";
            response.IsSuccess = true;
            response.Result = true;

            return response;
        }

        public async Task<ResponseData<List<AllowedExtensionToCreatorResponseModel>>> GetAllowedExtensionToCreatorAsync(Guid RoleId, CancellationToken cancellation)
        {
            var response = new ResponseData<List<AllowedExtensionToCreatorResponseModel>>();
            if (RoleId == Guid.Parse("4FF01FAB-90B3-4D90-EF35-08DE0A1A8CAA"))
                return response;

            response.Message = "Couldn't able to get the extension as per assigned";

            var getExtensions = await _context.AllowedExtensionOnRoles.Where(x => x.RoleId == RoleId).Select(x => x.FileTypeId).ToListAsync();
            if (getExtensions.Any())
            {
                var getExtensionDetails = await _context.AllowedFileTypesAndExtensions.Where(x => getExtensions.Contains(x.Id))
                .Select(x => new AllowedExtensionToCreatorResponseModel
                {
                   FileTypeId = x.Id,
                   FileExtension = x.FileExtension,
                   MinimumSize = x.MinimumSize ?? 0,
                   MaximumSize = x.MaximumSize ?? 0
                })
                .ToListAsync(cancellation);
                if(getExtensionDetails.Any())
                {
                    response.StatusCode = 200;
                    response.Message = "Allowed extensions fetched successfully";
                    response.Result = getExtensionDetails;
                    response.IsSuccess = true;
                } 
            }

            return response;
        }
        public async Task<ResponseData<List<GetPostResponseModel>>> GetPostAsync(CancellationToken cancellation)
        {
            var response = new ResponseData<List<GetPostResponseModel>>();

            var getPost = await _context.PostedContent.Select(x => new {
                UserId = x.UserId, 
                PostDescription = x.PostDescription,
                Media = x.MediaUrl})
            .ToListAsync(cancellation);
            if (getPost.Any())
            {
                response.StatusCode = 200;
                response.Message = "Got post successfully";
                response.Result = getPost;
                response.IsSuccess = true;
            }
            return response;
        }
    }
}
