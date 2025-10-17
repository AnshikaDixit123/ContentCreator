namespace ContentCreator.Domain.Entities.Identity
{
    public class PostLikes
    {
        public Guid Id { get; set; }
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
        public DateTime LikedAt { get; set; } = DateTime.Now;
        public bool IsLiked { get; set; } = false;
    }
}
