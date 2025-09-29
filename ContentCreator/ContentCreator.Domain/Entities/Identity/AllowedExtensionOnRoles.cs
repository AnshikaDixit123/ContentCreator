namespace ContentCreator.Domain.Entities.Identity
{
    public class AllowedExtensionOnRoles
    {
        public Guid Id { get; set; }
        public Guid RoleId { get; set; }
        public Guid FileTypeId { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
    }
}
