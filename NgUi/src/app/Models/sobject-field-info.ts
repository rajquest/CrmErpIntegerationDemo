export interface SObjectFieldInfo {
  name: string;
  label: string;
  type: string;
  length: number | null;
  precision: number | null;
  scale: number | null;
  updateable: boolean;
  nillable: boolean;
  custom: boolean;
  restrictedPicklist: boolean;
  picklistValues: string[] | null;
}
