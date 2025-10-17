namespace ContentCreator.Application.Common.DTOs.RequestDTOs
{
    public class PostLikesRequestModel
    {
        public Guid UserId {  get; set; }
        public Guid PostId {  get; set; }
        public DateTime LikedAt {  get; set; } =DateTime.Now;
        public bool IsLiked {  get; set; } 
    }
}
