namespace ContentCreator.Domain.Entities.Identity
{
    public class Country
    {
        public Guid Id { get; set; }
        public string CountryName { get; set; } = string.Empty;
        public string? CountryCode { get; set; }
        public string? PhoneCode { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }    
}
