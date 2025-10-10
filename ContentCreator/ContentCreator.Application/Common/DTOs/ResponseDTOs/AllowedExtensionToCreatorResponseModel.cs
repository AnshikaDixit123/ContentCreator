namespace ContentCreator.Application.Common.DTOs.ResponseDTOs
{
    public class AllowedExtensionToCreatorResponseModel
    {
        public Guid FileTypeId {  get; set; }
        public string FileExtension { get; set; }
        public decimal MinimumSize { get; set; }
        public decimal MaximumSize { get; set; }
    }
}
