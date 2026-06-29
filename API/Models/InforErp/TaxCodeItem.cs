namespace API.Models.InforErp
{
    public class TaxCodeItem
    {
        public string TaxCodeType { get; set; }
        public string TaxCode { get; set; }
        public string TaxRate { get; set; }
        public string TaxJur { get; set; }
        public string Description { get; set; }
        public string NxtLvlCode { get; set; }
        public string NxtLvlDescription { get; set; }

        // Item ID can stay as string since it's a combined metadata text
        public string _ItemId { get; set; }
    }

}
