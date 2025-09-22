namespace ContentCreator.Application.Common.DTOs.ResponseDTOs
{
    public class StateResponseModel
    {
        public Guid Id { get; set; }
        public string StateName { get; set; } = string.Empty;
        public string StateCode { get; set; } = string.Empty;
        public int CityCount { get; set; } = 0;
        public Guid CountryId { get; set; }
    }
}
