namespace ContentCreator.Application.Common.DTOs.ResponseDTOs
{
    public class CountryStateCityNestedResponseModel
    {
        public Guid CountryId { get; set; }
        public string CountryName { get; set; }
        public List<StateCityNestedResponseModel> StateList { get; set; } = new List<StateCityNestedResponseModel>();
    }
    public class StateCityNestedResponseModel
    {
        public Guid StateId { get; set; }
        public string StateName { get; set; }
        public List<CityNestedResponseModel> CityList { get; set; } = new List<CityNestedResponseModel>();
    }
    public class CityNestedResponseModel
    {
        public Guid CityId { get; set; }
        public string CityName { get; set; }
    }
}
