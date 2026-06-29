using System.Text.Json.Serialization;

namespace API.Models.InforErp
{
    public class JobOrderItem
    {
        public string? DerJob { get; set; }
        public string? Item { get; set; }
        public string? ItemDescription { get; set; }
        public string? Stat { get; set; }
        public string? Rework { get; set; } 
        public string? OrdRelease { get; set; }
        public string? QtyReleased { get; set; }
        public string? QtyComplete { get; set; }
        public string? QtyScrapped { get; set; }
        public string? JobDate { get; set; }
        public string? JschEndDate { get; set; }
        public string? ItemId { get; set; }
    }
}
