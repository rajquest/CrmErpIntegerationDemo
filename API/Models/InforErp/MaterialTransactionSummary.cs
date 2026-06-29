namespace API.Models.InforErp
{
    public class MaterialTransactionSummary
    {
        public string? MonthYear { get; set; }
        public string Item { get; set; } = string.Empty;
        public string ItemDescription { get; set; } = string.Empty;
        public decimal TotalQty { get; set; }
    }
}
