namespace ContentCreator.Application.Common.DTOs.RequestDTOs
{
    public class AddExtensionsRequest
    {
        public string FileType { get; set; }
        public string? FileExtension { get; set; }
        public int? MinimumSize { get; set; }
        public int? MaximumSize { get; set; }
    }
}
