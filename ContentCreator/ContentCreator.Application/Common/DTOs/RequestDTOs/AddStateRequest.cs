namespace ContentCreator.Application.Common.DTOs.RequestDTOs
{
    public class AddStateRequest
    {
        public string StateName { get; set; }
        public string StateCode { get; set; }
        public Guid CountryId { get; set; }
    }
}
