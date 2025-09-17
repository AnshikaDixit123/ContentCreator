namespace ContentCreator.Application.Common.DTOs.ResponseDTOs
{
    public class CityResponseModel
    {
        public Guid Id { get; set; }
        public string CityName { get; set; }
        public Guid CountryId { get; set; }
        public Guid StateId { get; set; }
    }
}
