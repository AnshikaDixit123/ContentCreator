namespace ContentCreator.Application.Common.DTOs.RequestDTOs
{
    public class PostCommentsRequestModel
    {
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
        public string Comment { get; set; }
        public DateTime CommentedAt { get; set; }
        public Guid? ParentId { get; set; }
    }
}
