namespace API.Models.InforErp
{
    public class InvoiceListingItem
    {
        // Invoice Number (Derived Invoice Number from Infor)
        public string DerInvNum { get; set; }

        // Customer Order Number (Derived from order header)
        public string DerCoNum { get; set; }

        // Invoice Date (Invoice Header Invoice Date)
        public string InvhdrInvDate { get; set; }

        // Total Invoice Price (Header Level Price Amount)
        public string InvhdrPrice { get; set; }

        // Item Code (Product / SKU Identifier)
        public string Item { get; set; }

        // Item Description (Derived Description of the Item)
        public string DerDescription { get; set; }

        // Order Line Number (Line Number within the Customer Order)
        public string CoLine { get; set; }

        // Customer Purchase Order Number
        public string DerCustPo { get; set; }
        public string CustaddrName { get; set; }
        public string InvhdrCustNum { get; set; }
        
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
        public string DerExtendedPrice { get; set; }
        public string Price { get; set; }
        
            
    }

}
