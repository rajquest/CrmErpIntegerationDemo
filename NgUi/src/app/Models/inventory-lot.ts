export interface InventoryLot {
  item: string;
  itemDescription: string;
  revision: string | null;
  expDate: string | null;
  derQtyOnHand: string;
  derExtUnitCost: string;
  itemU_M: string;
  lot: string;
  _itemId: string;
}
