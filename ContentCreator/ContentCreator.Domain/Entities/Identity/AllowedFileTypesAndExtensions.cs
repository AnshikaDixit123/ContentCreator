namespace ContentCreator.Domain.Entities.Identity
{
    public  class AllowedFileTypesAndExtensions
    {
        public Guid Id { get; set; }
        public string FileType {  get; set; }
        public string? FileExtension {  get; set; }
        public decimal? MinimumSize {  get; set; }
        public decimal? MaximumSize {  get; set; }
        public bool IsActive {  get; set; } = false;
    }
}
