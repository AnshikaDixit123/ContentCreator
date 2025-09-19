namespace ContentCreator.Application.Common.DTOs.ResponseDTOs
{
    public class UserResponseModel
    {
        public Guid UserId { get; set; }
        public string UserName {  get; set; }
        public string EmailAddress { get; set; }
        public string FirstName {  get; set; } = string.Empty;
        public string LastName {  get; set; } = string.Empty;
        public string PhoneNumber {  get; set; }
        public string CompleteAddress { get; set; } = string.Empty;
        public Guid? CountryId {  get; set; }
        public Guid? StateId {  get; set; }
        public Guid? CityId {  get; set; }
    }
}
