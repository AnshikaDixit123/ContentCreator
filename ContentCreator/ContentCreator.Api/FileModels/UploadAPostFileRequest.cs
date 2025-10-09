namespace ContentCreator.Api.FileModels
{
    public class UploadAPostFileRequest
    {
        public Guid UserId { get; set; }
        public string PostDescription { get; set; }
        public IFormFile File { get; set; }
        public string Visibility { get; set; }
        //public bool IsPublic { get; set; } = false;
        //public bool IsPrivate { get; set; } = false;
        //public bool IsSubscribed { get; set; } = false;
    }
}
