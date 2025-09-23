namespace ContentCreator.Application.Common.DTOs.RequestDTOs
{
    public class AddCityRequest
    {
        public string CityName { get; set; }
        public Guid CountryId { get; set; }
        public Guid StateId { get; set; }
    }
}
