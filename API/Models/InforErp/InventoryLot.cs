namespace API.Models.InforErp
{
    public class InventoryLot
    {
        public string Item { get; set; }
        public string ItemDescription { get; set; }
        public string Revision { get; set; }
        public string ExpDate { get; set; }
        public string DerQtyOnHand { get; set; }
        public string DerExtUnitCost { get; set; }
        public string ItemU_M { get; set; }
        public string Lot { get; set; }
        public string _ItemId { get; set; }
    }
}