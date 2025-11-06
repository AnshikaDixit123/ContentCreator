namespace ContentCreator.Application.Common.DTOs.RequestDTOs
{
    public class ReshareRequestModel
    {
        public Guid SharedBy {  get; set; }
        public DateTime SharedOn {  get; set; } = DateTime.Now;
        public Guid ParentId {  get; set; }
    }
}
