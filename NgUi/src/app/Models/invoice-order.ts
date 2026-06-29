import { TaxJurisdictionDetails } from './tax-jurisdiction-details';

export interface InvoicedOrder {
  derInvNum: string;
  derCoNum: string;
  invhdrInvDate: string;
  invhdrPrice: string;
  item: string;
  derDescription: string;
  coLine: string;
  derCustPo: string;
  qtyInvoiced: string;
  invhdrMiscCharges: string;
  invhdrPrepaidAmt: string;
  invhdrFreight: string;
  coNum: string;
  derCustNum?: string;
  coOrderDate?: string;
  shipDate?: string;
  shipToState?: string;
  shipToTaxCode?: string;
  stat?: string;
  customerName?: string;
  notes?: string;
  totalSalesTaxRate?: string;
  salesTax?: string;
  taxableAmount?: string;
  exemptAmount?: string;
  taxRateList: TaxJurisdictionDetails[];
  taxRateListDelimited: string;
}
