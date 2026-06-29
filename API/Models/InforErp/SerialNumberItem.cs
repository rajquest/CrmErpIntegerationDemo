namespace API.Models.InforErp
{
    public class SerialNumberItem
    {
        public string Item { get; set; } = default!;
        public string? Stat { get; set; } = default!;
        public string? SerNum { get; set; } = default!;
        public string? Loc { get; set; }
        public string? Lot { get; set; } = default!;
        public string? ExpDate { get; set; }
        public string? Whse { get; set; } = default!;
        public string _ItemId { get; set; } = default!;
    }
}
