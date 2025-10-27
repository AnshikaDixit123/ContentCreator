namespace ContentCreator.Application.Common.DTOs.ResponseDTOs
{
    public class GetPostResponseModel
    {
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string PostDescription { get; set; }
        public string Media { get; set; }
        public int LikeCount { get; set; }
        public bool IsLiked { get; set; }
    }
}
