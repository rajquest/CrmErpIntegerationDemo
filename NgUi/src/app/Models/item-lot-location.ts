export interface ItemLotLocation {
  item: string;
  itemDescription: string;
  lotRevision: string | null;
  qtyOnHand: string;
  itemUM: string;
  unitCost: string;
  loc: string;
  locationDescription: string;
  wbLotExpDate: string | null;
  whse: string;
  whseName: string;
  itemSerialTracked: string;
  lot: string;
  wbLotCreateDate: string | null;
  _ItemId: string;
  serialNumbers: string | null;
  expiryDates: string | null;
}
