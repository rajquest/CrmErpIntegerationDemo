using API.Common;

namespace API.Models.InforErp
{
    public class MaterialTransactionItem
    {
        public string? TransDate { get; set; }

        public string? TransType { get; set; }

        public DateTime? TransactionDate
        {
            get
            {
                return StringUtils.ParseToDateTime(TransDate);
            }
        }
        public string? RefType { get; set; }

        public string? Item { get; set; }
        public string? ItemDescription { get; set; }

        public string? Lot { get; set; }

        private string? _qty;
        public string? Qty
        {
            get => _qty;
            set => _qty = value?.Replace("-", "");
        }

        public string? _ItemId { get; set; }
    }
}
