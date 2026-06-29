using System.Globalization;

namespace API.Models.InforErp
{
    public class ItemLotLocationSerialNumbers
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
        public string? SerialNumbers { get; set; } = default!;
        public string? ExpiryDates { get; set; }

        // use these for date filters
        public DateTime? LotExpiryDate
        {
            get
            {
                if (string.IsNullOrWhiteSpace(WBLotExpDate))
                    return null;

                if (DateTime.TryParseExact(
                        WBLotExpDate,
                        "yyyyMMdd HH:mm:ss.fff",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out var date))
                {
                    return date;
                }

                return null;
            }
        }
        public DateTime? SerialNbrExpiryDate { get; set; } 
    }
}
