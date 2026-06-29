namespace API.Models.InforErp
{
    public class ItemLotLocation
    {
        public string Item { get; set; } = default!;
        public string ItemDescription { get; set; } = default!;
        public string? LotRevision { get; set; }

        public string? QtyOnHand { get; set; }
        public string? ItemUM { get; set; } = default!;
        public string? UnitCost { get; set; }

        public string Loc { get; set; } = default!;
        public string LocationDescription { get; set; } = default!;

        public string? WBLotExpDate { get; set; }

        public string Whse { get; set; } = default!;
        public string WhseName { get; set; } = default!;

        /// <summary>
        /// "0" or "1" from API — treat as bool at service layer if needed
        /// </summary>
        public string ItemSerialTracked { get; set; } = default!;

        public string Lot { get; set; } = default!;
        public string? WBLotCreateDate { get; set; }

        public string _ItemId { get; set; } = default!;
    }
}
