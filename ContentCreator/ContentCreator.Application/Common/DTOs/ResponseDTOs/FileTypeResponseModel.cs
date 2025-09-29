namespace ContentCreator.Application.Common.DTOs.ResponseDTOs
{
    public class FileTypeResponseModel
    {
        public Guid Id { get; set; }
        public string FileType { get; set; } = string.Empty;
        public string? FileExtension { get; set; }
        public int? MinimumSize { get; set; }
        public int? MaximumSize { get; set; }
        public bool IsActive { get; set; } = false;
    }
}
