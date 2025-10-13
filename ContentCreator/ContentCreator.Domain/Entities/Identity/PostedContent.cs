namespace ContentCreator.Domain.Entities.Identity
{
    public class PostedContent
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string PostDescription {  get; set; }
        public string? MediaUrl {  get; set; }
        public DateTime DatePosted {  get; set; } = DateTime.Now;
        public bool IsPublic {  get; set; } = true;
        public bool IsPrivate {  get; set; } = false;
        public bool IsSubscribed {  get; set; } = false;
    }
}
