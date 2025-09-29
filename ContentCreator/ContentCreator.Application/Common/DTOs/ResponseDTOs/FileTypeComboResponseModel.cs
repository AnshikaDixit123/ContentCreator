namespace ContentCreator.Application.Common.DTOs.ResponseDTOs
{
    public class FileTypeComboResponseModel
    {
        public List<OnlyFileTypeResponseModel> FileTypeList { get; set; } = new List<OnlyFileTypeResponseModel>();
        public List<FileTypeResponseModel> FileTypeDetailList { get; set; } = new List<FileTypeResponseModel>();
    }
}
