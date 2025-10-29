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
        public async Task<ResponseData<List<GetPostResponseModel>>> GetPostAsync(Guid userId, CancellationToken cancellation)
        {
            var response = new ResponseData<List<GetPostResponseModel>>();

            string scheme = _httpContextAccessor.HttpContext.Request.Scheme;
            string host = _httpContextAccessor.HttpContext.Request.Host.Value;
            string hostedUrl = $"{scheme}://{host}/";

            var getPost = await _context.PostedContent
                .Select(x => new GetPostResponseModel
                {
                    PostId = x.Id,
                    UserId = x.UserId,
                    PostDescription = x.PostDescription,
                    //LikeCount = x.LikeCount,
                    Media = !string.IsNullOrEmpty(x.MediaUrl)
                        ? Path.Combine(hostedUrl, x.MediaUrl.Replace("\\", "/"))
                        : null
                })
                        .ToListAsync(cancellation);

            List<Guid> postIds = getPost.Select(x => x.PostId).ToList();
            List<Guid> userIds = getPost.Select(x => x.UserId).Distinct().ToList();

            // Step 3: Fetch users in one go
            var users = await _userManager.Users
                .Where(u => userIds.Contains(u.Id))
                .Select(u => new { u.Id, u.UserName })
                .ToListAsync(cancellation);

            var likeData = await _context.PostLikes
        .Where(pl => postIds.Contains(pl.PostId))
        .GroupBy(pl => pl.PostId)
        .Select(g => new { PostId = g.Key, Count = g.Count() })
        .ToListAsync(cancellation);
            var existingLikes = await _context.PostLikes.Where(x => x.UserId == userId).Select(x => new {x.UserId, x.IsLiked, x.PostId}).ToListAsync(cancellation);
            // Step 4: Project posts (no foreach)
            getPost = getPost
                .Select(p => new GetPostResponseModel
                {
                    UserId = p.UserId,
                    PostId = p.PostId,
                    PostDescription = p.PostDescription,
                    Media = p.Media,
                    LikeCount = likeData.FirstOrDefault(ld => ld.PostId == p.PostId)?.Count ?? 0,
                    UserName = users.FirstOrDefault(u => u.Id == p.UserId)?.UserName ?? "Unknown User",
                    IsLiked = existingLikes.Any(l => l.PostId == p.PostId && l.IsLiked)
                })
                .ToList();

            // Step 5: Prepare response
            response.StatusCode = 200;
            response.Message = "Got posts successfully";
            response.Result = getPost;
            response.IsSuccess = true;

            return response;
        }
        public async Task<ResponseData<bool>> PostLikesAsync(PostLikesRequestModel request, CancellationToken cancellation)
        {
            var response = new ResponseData<bool>();
            response.Message = "Post unliked";

            var getPost = await _context.PostedContent.Where(x => x.Id == request.PostId).FirstOrDefaultAsync();
            if(getPost != null)
            {
                var existingLike = await _context.PostLikes.FirstOrDefaultAsync(x => x.PostId == request.PostId && x.UserId == request.UserId, cancellation);
                var likeCount = await _context.PostedContent.CountAsync(l => l.Id == request.PostId);
                if (existingLike == null)
                {

                    var like = new PostLikes();
                    like.PostId = request.PostId;
                    like.UserId = request.UserId;
                    like.LikedAt = DateTime.Now;
                    like.IsLiked = request.IsLiked;
                    _context.PostLikes.Add(like);

                    getPost.LikeCount = getPost.LikeCount + 1;

                    response.StatusCode = 200;
                    response.Message = "Post liked successfully";
                    response.Result = true;
                    response.IsSuccess = true;
                }
                else
                {
                    if (request.IsLiked == false)
                    {
                        _context.PostLikes.Remove(existingLike);

                        getPost.LikeCount = Math.Max(0, getPost.LikeCount - 1);

                        response.StatusCode = 200;
                        response.Message = "Like removed successfully";
                        response.Result = true;
                        response.IsSuccess = true;
                    }
                }

                await _context.SaveChangesAsync(cancellation);
            }

            return response;
        }

    }
}
