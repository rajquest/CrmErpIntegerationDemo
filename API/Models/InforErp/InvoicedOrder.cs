namespace API.Models.InforErp
{
    public class InvoicedOrder
    {
        // Invoice Number (Derived Invoice Number from Infor)
        public string DerInvNum { get; set; }

        // Customer Order Number (Derived from order header)
        public string DerCoNum { get; set; }

        // Invoice Date (Invoice Header Invoice Date)
        public string InvhdrInvDate { get; set; }

        // Total Invoice Price (Header Level Price Amount)
        public string InvhdrPrice { get; set; }
        public string InvhdrCustNum { get; set; }
        public string CustaddrName { get; set; }

        // Item Code (Product / SKU Identifier)
        public string Item { get; set; }

        // Item Description (Derived Description of the Item)
        public string DerDescription { get; set; }

        // Order Line Number (Line Number within the Customer Order)
        public string CoLine { get; set; }
        // Customer Purchase Order Number
        public string DerCustPo { get; set; }
        // Quantity Invoiced for this Line Item
        public string QtyInvoiced { get; set; }
        // Miscellaneous Charges applied to the Invoice
        public string InvhdrMiscCharges { get; set; }
        // Prepaid Amount applied to the Invoice
        public string InvhdrPrepaidAmt { get; set; }
        // Freight Charges applied to the Invoice
        public string InvhdrFreight { get; set; }
        // Customer Order Number (Original Order Reference)
        public string CoNum { get; set; }
        
        // Order and Shipto Data
        public string? DerCustNum { get; set; }
        public string? CoOrderDate { get; set; }
        public string? ShipDate { get; set; }
        public string? ShipToState { get; set; }
        public string? ShipToTaxCode { get; set; }
        public string? Stat { get; set; }
        public string? CustomerName { get; set; }
        public string? Notes { get; set; }
        public decimal? TotalSalesTaxRate { get; set; }
        public string? SalesTax { get; set; }
        public string? TaxableAmount { get; set; }
        public string? ExemptAmount { get; set; }
        public TaxJurisdictionDetails[] TaxRateList { get; set; }
    }
}
