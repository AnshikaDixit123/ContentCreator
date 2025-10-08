namespace ContentCreator.Api.FileModels
{
    public class UploadAPostFileRequest
    {
        public Guid UserId { get; set; }
        public string PostDescription { get; set; }
        public IFormFile File { get; set; }
    }
}
