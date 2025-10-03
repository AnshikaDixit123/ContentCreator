namespace ContentCreator.Application.Common.DTOs.RequestDTOs
{
    public class AssignExtensionsRequest
    {
        public Guid RoleId { get; set; }
        public List<Guid> FileTypeId { get; set; } = new List<Guid>();
    }
}
