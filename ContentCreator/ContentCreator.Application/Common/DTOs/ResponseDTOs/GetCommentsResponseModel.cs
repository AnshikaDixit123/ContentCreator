namespace ContentCreator.Application.Common.DTOs.ResponseDTOs
{
    public class GetCommentsResponseModel
    {
        public Guid PostId { get; set; }
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public Guid? ParentId { get; set; }
        public string Comment { get; set; }
        public DateTime CommentedAt { get; set; }
    }
}
