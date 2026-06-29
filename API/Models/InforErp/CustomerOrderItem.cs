namespace API.Models.InforErp
{
    public class CustomerOrderItem
    {
        public string CoNum { get; set; }
        public string DerCustNum { get; set; }
        public string CoType { get; set; }
        public string DerCustPo { get; set; }

        public string CoOrderDate { get; set; }   
        public string QtyOrdered { get; set; }
        public string Stat { get; set; }
        public string ShipDate { get; set; }

        public string CustSeq { get; set; }
        public string CoPrice { get; set; }

        public string Item { get; set; }
        public string CoLine { get; set; }
        public string _ItemId { get; set; }
    }

}
