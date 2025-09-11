namespace ContentCreator.Application.Common.DTOs
{
    public class UserExistsDto
    {
        public bool EmailExists { get; set; }
        public bool UserNameExists { get; set; }
        public bool PhoneExists { get; set; }
    }
}
