import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, defer, map } from 'rxjs';
import { environment } from '../../environments/environment';
import { ConfigurationResponse } from '../Models/configuration-response';
import { CustomerOrder } from '../Models/customer-order';
import { Item } from '../Models/items';
import { InventoryLot } from '../Models/inventory-lot';
import { ItemLotLocation } from '../Models/item-lot-location';
import { Customer } from '../Models/customer';
import { TaxCodeItem } from '../Models/tax-codes';
import { TaxGroupSummary } from '../Models/tax-group-summary.model';
import { SerialNumberItem } from '../Models/serial-number-item';
import { TokenService } from './token.service';
import { BaseService } from './base.service';
import { JobVariance } from '../Models/job-variance';
import { InvoicedOrder } from '../Models/invoice-order';
import { MaterialTransactionSummary } from '../Models/material-transaction-summary';

@Injectable({
  providedIn: 'root'
})
export class InforApiService extends BaseService {
  private baseApiUrl = `${environment.apiBaseUrl}`;

  constructor(http: HttpClient,
    private tokenService: TokenService) {
    super(http);
  }

  getConfigurations(): Observable<string[]> {
    return defer(() => {
      const token = this.tokenService.getInforApiToken();
      const url = `${this.baseApiUrl}/InforExplorer/GetConfigurations?bearerToken=${encodeURIComponent(token)}`;
      const headers = new HttpHeaders({ 'Accept': 'application/json' });
      return this.http.post<string[]>(url, null, { headers });
    }).pipe(map(res => res || []));
  }

  getTableAttributesList(tableName: string): Observable<string[]> {
    const url = `${this.baseApiUrl}/InforExplorer/GetIdoTableSchema?tableName=${tableName}`;
    return this.postWithToken<string[]>(url);
  }

  getSLItems(rowCount: Number): Observable<Item[]> {
    const url = `${this.baseApiUrl}/InforExplorer/GetSLItems?rowCount=${rowCount}`;
    return this.postWithToken<Item[]>(url);
  }

  getCustomerOrders(rowCount: number, filter?: string): Observable<CustomerOrder[]> {
    let url = `${this.baseApiUrl}/Customer/GetCustomerOrders?rowCount=${rowCount}`;
    if (filter) {
      url += `&filter=${encodeURIComponent(filter)}`;
    }
    return this.postWithToken<CustomerOrder[]>(url);
  }

  getCustomers(rowCount: number, filter?: string): Observable<Customer[]> {
    let url = `${this.baseApiUrl}/Customer/GetCustomers?rowCount=${rowCount}`;
    if (filter) {
      url += `&filter=${encodeURIComponent(filter)}`;
    }
    return this.postWithToken<Customer[]>(url);
  }

  getInventoryLots(rowCount: number): Observable<InventoryLot[]> {
    const url = `${this.baseApiUrl}/Inventory/GetInventoryLots?rowCount=${rowCount}`;
    return this.postWithToken<InventoryLot[]>(url);
  }

  getSerialNumbers(rowCount: number, filter: string): Observable<SerialNumberItem[]> {
    let url = `${this.baseApiUrl}/Inventory/GetSerialNumbers?rowCount=${rowCount}`;
    if (filter) {
      url += `&filter=${encodeURIComponent(filter)}`;
    }
    return this.postWithToken<SerialNumberItem[]>(url);
  }

  getItemLotLocation(rowCount: number, filter: string | undefined): Observable<ItemLotLocation[]> {
    let url = `${this.baseApiUrl}/Inventory/GetItemLotLocation?rowCount=${rowCount}`;
    if (filter && filter.trim().length > 0) {
      url += `&filter=${encodeURIComponent(filter)}`;
    }
    return this.postWithToken<ItemLotLocation[]>(url);
  }

  getItemLotLocationWithSerialNbrsMapping(rowCount: number,
    filter?: string,
    startDate?: string,
    endDate?: string,
    hasExpiryDateConflict?: boolean): Observable<ItemLotLocation[]> {
    let url = `${this.baseApiUrl}/Inventory/GetItemLotSerialNumbers?rowCount=${rowCount}`;
    if (filter && filter.trim().length > 0) {
      url += `&filter=${encodeURIComponent(filter)}`;
    }
    if (startDate) {
      url += `&startDate=${encodeURIComponent(startDate)}`;
    }
    if (endDate) {
      url += `&endDate=${encodeURIComponent(endDate)}`;
    }
    if (hasExpiryDateConflict) {
      url += `&hasExpiryDateConflict=${encodeURIComponent(hasExpiryDateConflict)}`;
    }
    return this.postWithToken<ItemLotLocation[]>(url);
  }

  getTaxCodes(rowCount: number): Observable<TaxCodeItem[]> {
    const url = `${this.baseApiUrl}/Tax/GetTaxCodes?rowCount=${rowCount}`;
    return this.postWithToken<TaxCodeItem[]>(url);
  }

  getJobVariance(rowCount: number, filterCriteria: string): Observable<JobVariance[]> {
    let url = `${this.baseApiUrl}/JobVariance/GetCostVariance?rowCount=${rowCount}`;
    if (filterCriteria) {
      url += `&filter=${encodeURIComponent(filterCriteria)}`;
    }
    return this.postWithToken<JobVariance[]>(url);
  }

  getInvoiceOrders(filterCriteria?: string): Observable<InvoicedOrder[]> {
    let url = `${this.baseApiUrl}/InvoiceOrder/InvoicedOrders`;
    if (filterCriteria) {
      url += `?filter=${encodeURIComponent(filterCriteria)}`;
    }
    return this.postWithToken<InvoicedOrder[]>(url);
  }

  getMaterialTransactionSummary(filterCriteria?: string): Observable<MaterialTransactionSummary[]> {
    let url = `${this.baseApiUrl}/InvoiceOrder/material-transactions`;
    if (filterCriteria) {
      url += `?filter=${encodeURIComponent(filterCriteria)}`;
    }
    return this.postWithToken<MaterialTransactionSummary[]>(url);
  }

  private postWithToken<T>(url: string): Observable<T> {
    return defer(() => {
      const token = this.tokenService.getInforApiToken();
      const headers = new HttpHeaders({
        'Accept': 'application/json',
        'Content-Type': 'application/json'
      });
      return this.post<T>(url, JSON.stringify(token), headers);
    }).pipe(map(res => res ?? ([] as unknown as T)));
  }
}
