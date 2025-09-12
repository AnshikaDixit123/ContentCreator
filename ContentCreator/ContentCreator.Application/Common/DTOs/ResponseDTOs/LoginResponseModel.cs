namespace ContentCreator.Application.Common.DTOs.ResponseDTOs
{
    public class LoginResponseModel
    {
        public Guid UserId {  get; set; }
        public string UserName {  get; set; }
        public string UserEmail {  get; set; }
        public string UserPhoneNumber {  get; set; }
        public string UserRole {  get; set; }
        public string RoleType {  get; set; }
        public string UserToken {  get; set; }
    }
}
