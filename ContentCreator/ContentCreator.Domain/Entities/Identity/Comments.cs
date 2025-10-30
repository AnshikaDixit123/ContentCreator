namespace ContentCreator.Domain.Entities.Identity
{
    public class Comments
    {
        public Guid Id { get; set; }
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
        public string Comment { get; set; }
        public DateTime CommentedAt { get; set; } = DateTime.Now;
        public Guid? ParentId { get; set; }
    }
}
