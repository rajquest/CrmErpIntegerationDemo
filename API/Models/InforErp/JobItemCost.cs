namespace API.Models.InforErp
{
    public class JobItemCost
    {
        public string? Item { get; set; } = default!;
        public string? ItemDescription { get; set; } = default!;
        public string? DerItmAsmSubtotal { get; set; } = default!;
        public string? QtyOnHand { get; set; } = default!;
        public string? WBDerUnitCost { get; set; } = default!; // Unit cost
        public string? _ItemId { get; set; } = default!; // Assembly Material Cost
    }
}
