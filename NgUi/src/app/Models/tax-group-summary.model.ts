export interface TaxGroupSummary {
  taxJurisdiction: string;
  taxCode: string;
  description: string;
  salesTaxRate: number;
  grossSales: number;
  tax: number;
}
