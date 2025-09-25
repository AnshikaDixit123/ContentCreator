namespace ContentCreator.Domain.Entities.Identity
{
    public  class AllowedFileTypesAndExtensions
    {
        public Guid Id { get; set; }
        public string FileType {  get; set; }
        public string? FileExtension {  get; set; }
        public int? MinimumSize {  get; set; }
        public int? MaximumSize {  get; set; }
        public bool IsActive {  get; set; } = false;
    }
}
