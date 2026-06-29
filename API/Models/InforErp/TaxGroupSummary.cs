namespace API.Models.InforErp
{
    public class TaxGroupSummary
    {
        public string TaxJurisdiction { get; set; }  
        public string TaxCode { get; set; }
        public string TaxDescription { get; set; }
        public decimal SalesTaxRate { get; set; }

        public decimal GrossSales { get; set; }        // LineTotal sum
        public decimal Tax { get; set; }     
    }
}
