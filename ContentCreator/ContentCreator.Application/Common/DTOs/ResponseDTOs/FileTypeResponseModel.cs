namespace ContentCreator.Application.Common.DTOs.ResponseDTOs
{
    public class FileTypeResponseModel
    {
        public Guid Id { get; set; }
        public string FileType { get; set; } = string.Empty;
        public string? FileExtension { get; set; }
        public decimal? MinimumSize { get; set; }
        public decimal? MaximumSize { get; set; }
        public bool IsActive { get; set; } = false;
    }
}
