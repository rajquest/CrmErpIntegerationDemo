import { BaseService } from './base.service';
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../environments/environment';
import { TokenService } from './token.service';
import { ItemList } from '../Models/item-list';
import { Warehouse } from '../Models/warehouse';
import { OrderNumber } from '../Models/order-number';

@Injectable({
  providedIn: 'root'
})
export class LookupService extends BaseService {
  private baseApiUrl = `${environment.apiBaseUrl}`;

  constructor(http: HttpClient,
    private tokenService: TokenService) {
    super(http);
  }

  getSLItemsList(): Observable<ItemList[]> {
    const url = `${this.baseApiUrl}/Lookup/GetItems`;
    return this.postWithToken<ItemList[]>(url);
  }

  getWarehouses(): Observable<Warehouse[]> {
    const url = `${this.baseApiUrl}/Lookup/GetWarehouses`;
    return this.postWithToken<Warehouse[]>(url);
  }

  getLocations(): Observable<Location[]> {
    const url = `${this.baseApiUrl}/Lookup/GetLocations`;
    return this.postWithToken<Location[]>(url);
  }

  getOrderNumbers(): Observable<OrderNumber[]> {
    const url = `${this.baseApiUrl}/Lookup/GetOrderNumbers`;
    return this.postWithToken<OrderNumber[]>(url);
  }

  private postWithToken<T>(url: string): Observable<T> {
    const token = this.tokenService.getInforApiToken();

    const headers = new HttpHeaders({
      'Accept': 'text/plain',
      'Content-Type': 'application/json'
    });

    return this.post<T>(url, JSON.stringify(token), headers)
      .pipe(map(res => res ?? ([] as unknown as T)));
  }
}
