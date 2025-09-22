namespace ContentCreator.Application.Common.DTOs.ResponseDTOs
{
    public class CountryResponseModel
    {
        public Guid Id { get; set; }
        public string CountryName { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty;
        public string PhoneCode { get; set; } = string.Empty;
        public int StateCount { get; set; } = 0;

    }
}
