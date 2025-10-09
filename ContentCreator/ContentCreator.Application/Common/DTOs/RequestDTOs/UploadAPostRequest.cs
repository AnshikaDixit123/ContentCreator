namespace ContentCreator.Application.Common.DTOs.RequestDTOs
{
    public class UploadAPostRequest
    {
        public Guid UserId {  get; set; }
        public string PostDescription { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string Visibility { get; set; }
        //public bool IsPublic { get; set; } = false;
        //public bool IsPrivate { get; set; } = false;
        //public bool IsSubscribed { get; set; } = false;
    }
}
