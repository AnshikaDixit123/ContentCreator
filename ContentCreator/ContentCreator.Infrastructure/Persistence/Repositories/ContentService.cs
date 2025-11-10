using Azure.Core;
using ContentCreator.Application.Common.DTOs.RequestDTOs;
using ContentCreator.Application.Common.DTOs.ResponseDTOs;
using ContentCreator.Application.Interfaces;
using ContentCreator.Domain.Entities.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


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
                if (getExtensionDetails.Any())
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

            // Get all posts including reshared ones
            var getPost = await _context.PostedContent
                .Select(x => new GetPostResponseModel
                {
                    PostId = x.Id,
                    UserId = x.UserId,
                    PostDescription = x.PostDescription,
                    Media = !string.IsNullOrEmpty(x.MediaUrl)
                        ? Path.Combine(hostedUrl, x.MediaUrl.Replace("\\", "/"))
                        : null,
                    SharedBy = x.SharedBy,
                    ParentId = x.ParentId,
                    DatePosted = x.DatePosted,
                    IsReshared = x.SharedBy != null
                })
                .ToListAsync(cancellation);

            List<Guid> postIds = getPost.Select(x => x.PostId).ToList();
            List<Guid> userIds = getPost.Select(x => x.UserId).Distinct().ToList();

            // Get original post data for reshared posts WITH MEDIA
            var parentPostIds = getPost.Where(x => x.ParentId.HasValue).Select(x => x.ParentId.Value).Distinct().ToList();
            var originalPosts = await _context.PostedContent
                .Where(x => parentPostIds.Contains(x.Id))
                .Select(x => new
                {
                    x.Id,
                    x.UserId,
                    x.PostDescription,
                    Media = !string.IsNullOrEmpty(x.MediaUrl)
                        ? Path.Combine(hostedUrl, x.MediaUrl.Replace("\\", "/"))
                        : null
                })
                .ToListAsync(cancellation);

            // Add original post user IDs to the user list
            var originalUserIds = originalPosts.Select(x => x.UserId).Distinct().ToList();
            userIds.AddRange(originalUserIds);
            userIds = userIds.Distinct().ToList();

            // Get all users (ONLY UserName - no FullName)
            var users = await _userManager.Users
                .Where(u => userIds.Contains(u.Id))
                .Select(u => new { u.Id, u.UserName })
                .ToListAsync(cancellation);

            var likeData = await _context.PostLikes
                .Where(pl => postIds.Contains(pl.PostId))
                .GroupBy(pl => pl.PostId)
                .Select(g => new { PostId = g.Key, Count = g.Count() })
                .ToListAsync(cancellation);

            var existingLikes = await _context.PostLikes.Where(x => x.UserId == userId).Select(x => new { x.UserId, x.IsLiked, x.PostId }).ToListAsync(cancellation);

            // Build the response with original post information
            getPost = getPost
                .Select(p => new GetPostResponseModel
                {
                    UserId = p.UserId,
                    PostId = p.PostId,
                    PostDescription = p.PostDescription,
                    Media = p.Media,
                    LikeCount = likeData.FirstOrDefault(ld => ld.PostId == p.PostId)?.Count ?? 0,
                    UserName = users.FirstOrDefault(u => u.Id == p.UserId)?.UserName ?? "Unknown User",
                    IsLiked = existingLikes.Any(l => l.PostId == p.PostId && l.IsLiked),
                    SharedBy = p.SharedBy,
                    ParentId = p.ParentId,
                    DatePosted = p.DatePosted,
                    IsReshared = p.IsReshared,
                    // Original post information for reshared posts
                    OriginalPost = p.ParentId.HasValue ? new OriginalPostModel
                    {
                        UserId = originalPosts.FirstOrDefault(op => op.Id == p.ParentId.Value)?.UserId ?? Guid.Empty,
                        UserName = users.FirstOrDefault(u => u.Id == originalPosts.FirstOrDefault(op => op.Id == p.ParentId.Value)?.UserId)?.UserName ?? "Unknown User",
                        PostDescription = originalPosts.FirstOrDefault(op => op.Id == p.ParentId.Value)?.PostDescription,
                        Media = originalPosts.FirstOrDefault(op => op.Id == p.ParentId.Value)?.Media
                    } : null
                })
                .OrderByDescending(x => x.DatePosted)
                .ToList();

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
            if (getPost != null)
            {
                var existingLike = await _context.PostLikes.FirstOrDefaultAsync(x => x.PostId == request.PostId && x.UserId == request.UserId, cancellation);

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
        public async Task<ResponseData<bool>> PostCommentsAsync(PostCommentsRequestModel request, CancellationToken cancellation)
        {
            var response = new ResponseData<bool>();

            var getPost = await _context.PostedContent
                .Where(x => x.Id == request.PostId)
                .FirstOrDefaultAsync(cancellation);

            if (getPost != null)
            {
                if (request.ParentId.HasValue)
                {
                    var parentComment = await _context.Comments
                        .Where(x => x.Id == request.ParentId.Value && x.PostId == request.PostId)
                        .FirstOrDefaultAsync(cancellation);

                    if (parentComment == null)
                    {
                        request.ParentId = null;
                    }
                }

                var comment = new Comments();
                comment.PostId = request.PostId;
                comment.UserId = request.UserId;
                comment.Comment = request.Comment;
                comment.ParentId = request.ParentId;
                _context.Comments.Add(comment);
                await _context.SaveChangesAsync(cancellation);

                response.StatusCode = 200;
                response.Result = true;
                response.Message = request.ParentId.HasValue ? "Reply posted successfully" : "Comment posted successfully";
                response.IsSuccess = true;
            }
            else
            {
                response.Message = "Post not found";
                response.StatusCode = 404;
                response.Result = false;
                response.IsSuccess = false;
            }

            return response;
        }
        public async Task<ResponseData<List<GetCommentsResponseModel>>> GetCommentsAsync(Guid postId, CancellationToken cancellation)
        {
            var response = new ResponseData<List<GetCommentsResponseModel>>();

            var getComments = await _context.Comments
                .Where(x => x.PostId == postId && x.ParentId == null)
                .OrderBy(x => x.CommentedAt)
                .Select(x => new GetCommentsResponseModel

                {
                    Id = x.Id,
                    PostId = x.PostId,
                    UserId = x.UserId,
                    ParentId = x.ParentId,
                    Comment = x.Comment,
                    CommentedAt = x.CommentedAt
                })
                .ToListAsync(cancellation);

            // Get usernames from UserManager for each comment
            if (getComments != null && getComments.Any())
            {

                foreach (var comment in getComments)
                {
                    var user = await _userManager.FindByIdAsync(comment.UserId.ToString());
                    comment.UserName = user?.UserName ?? "Unknown User";
                }

                response.StatusCode = 200;
                response.Result = getComments;
                response.Message = "Comments fetched successfully";
                response.IsSuccess = true;
            }
            else
            {
                response.StatusCode = 200;
                response.Result = new List<GetCommentsResponseModel>();
                response.Message = "No comments found";
                response.IsSuccess = true;
            }
            return response;
        }

        public async Task<ResponseData<List<GetCommentsResponseModel>>> GetRepliesAsync(Guid commentId, CancellationToken cancellation)
        {
            var response = new ResponseData<List<GetCommentsResponseModel>>();

            var getReplies = await _context.Comments
                .Where(x => x.ParentId == commentId)
                .OrderBy(x => x.CommentedAt)
                .Select(x => new GetCommentsResponseModel
                {
                    Id = x.Id,
                    PostId = x.PostId,
                    UserId = x.UserId,
                    ParentId = x.ParentId,
                    Comment = x.Comment,
                    CommentedAt = x.CommentedAt
                })
                .ToListAsync(cancellation);

            if (getReplies != null && getReplies.Any())
            {
                foreach (var reply in getReplies)
                {
                    var user = await _userManager.FindByIdAsync(reply.UserId.ToString());
                    reply.UserName = user?.UserName ?? "Unknown User";
                }

                response.StatusCode = 200;
                response.Result = getReplies;
                response.Message = "Replies fetched successfully";
                response.IsSuccess = true;
            }
            else
            {
                response.StatusCode = 200;
                response.Result = new List<GetCommentsResponseModel>();
                response.Message = "No replies found";
                response.IsSuccess = true;
            }
            return response;
        }
        public async Task<ResponseData<bool>> ReshareAsync(ReshareRequestModel request, CancellationToken cancellation)
        {
            var response = new ResponseData<bool>();
            var orignalPost = await _context.PostedContent
                .Where(x => x.Id == request.ParentId)
                .FirstOrDefaultAsync();

            if (orignalPost != null)
            {
                var reshare = new PostedContent();
                reshare.UserId = request.SharedBy; // Person who reshared
                reshare.PostDescription = orignalPost.PostDescription;
                reshare.MediaUrl = orignalPost.MediaUrl;
                reshare.DatePosted = DateTime.Now;
                reshare.IsPublic = true;
                reshare.IsPrivate = false;
                reshare.IsSubscribed = false;
                reshare.LikeCount = 0;
                reshare.SharedBy = request.SharedBy;
                reshare.SharedOn = DateTime.Now;
                reshare.ParentId = request.ParentId;

                _context.PostedContent.Add(reshare);
                await _context.SaveChangesAsync(cancellation);

                response.Message = "Post shared successfully.";
                response.StatusCode = 200;
                response.IsSuccess = true;
                response.Result = true;
            }
            else
            {
                response.Message = "Original post not found.";
                response.IsSuccess = false;
                response.Result = false;
            }
            return response;
        }
    }
}
