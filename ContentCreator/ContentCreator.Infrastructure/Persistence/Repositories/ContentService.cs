using Azure.Core;
using ContentCreator.Application.Common.DTOs.RequestDTOs;
using ContentCreator.Application.Common.DTOs.ResponseDTOs;
using ContentCreator.Application.Interfaces;
using ContentCreator.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;


namespace ContentCreator.Infrastructure.Persistence.Repositories
{
    public class ContentService : IContentService
    {
        private readonly IContentCreatorDBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ContentService(IContentCreatorDBContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
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

            string scheme = _httpContextAccessor.HttpContext.Request.Scheme;
            string host = _httpContextAccessor.HttpContext.Request.Host.Value;
            string hostedUrl = $"{scheme}://{host}/";

            var getPost = await _context.PostedContent
                .Select(x => new GetPostResponseModel
                {
                    UserId = x.UserId,
                    PostDescription = x.PostDescription,
                    Media = !string.IsNullOrEmpty(x.MediaUrl)
                        ? Path.Combine(hostedUrl, x.MediaUrl.Replace("\\", "/"))
                        : null
                })
                        .ToListAsync(cancellation);

            List<Guid> userIds = getPost.Select(x => x.UserId).Distinct().ToList();

            // Step 3: Fetch users in one go
            var users = await _userManager.Users
                .Where(u => userIds.Contains(u.Id))
                .Select(u => new { u.Id, u.UserName })
                .ToListAsync(cancellation);

            // Step 4: Project posts (no foreach)
            getPost = getPost
                .Select(p => new GetPostResponseModel
                {
                    UserId = p.UserId,
                    PostDescription = p.PostDescription,
                    Media = p.Media,
                    LikeCount = p.LikeCount,
                    UserName = users.FirstOrDefault(u => u.Id == p.UserId)?.UserName ?? "Unknown User"
                })
                .ToList();

            // Step 5: Prepare response
            response.StatusCode = 200;
            response.Message = "Got posts successfully";
            response.Result = getPost;
            response.IsSuccess = true;

            return response;
        }

    }
}
