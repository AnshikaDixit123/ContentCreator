namespace ContentCreator.Application.Common.DTOs.ResponseDTOs
{
    public class RolesResponseModel
    {
        public Guid Id { get; set; }
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
        public string RoleType { get; set; }
        public int UserCount { get; set; }
        public bool IsProtected { get; set; }
    }
}
