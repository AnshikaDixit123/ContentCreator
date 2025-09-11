namespace ContentCreator.Application.Common.DTOs.RequestDTOs
{
    public class CreateNewUserRequest
    {
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public Guid RoleID { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
