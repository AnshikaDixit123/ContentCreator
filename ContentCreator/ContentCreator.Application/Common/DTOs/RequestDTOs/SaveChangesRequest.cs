namespace ContentCreator.Application.Common.DTOs.RequestDTOs
{
    public class SaveChangesRequest
    {
        public Guid UserId { get; set; }
        //public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string CompleteAddress { get; set; }
        public Guid CountryId { get; set; }
        public Guid StateId { get; set; }
        public Guid CityId { get; set; }
    }
}
