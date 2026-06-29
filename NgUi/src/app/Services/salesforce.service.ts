import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, catchError, map, throwError } from 'rxjs';
import { SObjectFieldInfo } from '../Models/sobject-field-info';
import { SalesforceProductRecord } from '../Models/salesforce-product-record';

@Injectable({
  providedIn: 'root'
})
export class SalesforceService {
  private baseApiUrl = `${environment.apiBaseUrl}/Salesforce`;
  constructor(private http: HttpClient) { }

  validateBearerToken(bearerToken: string | null): string | Observable<never> {
    if (!bearerToken) {
      console.error('Missing Bearer token. Please authenticate first.');
      return throwError(() => new Error('Missing Bearer token. Please authenticate first.'));
    }
    return bearerToken;
  }

  getObjectsList(): Observable<string[]> {
    const bearerToken = sessionStorage.getItem('sf_access_token');
    this.validateBearerToken(bearerToken);
    const url = `${this.baseApiUrl}/GetAllsObjects`;
    const headers = new HttpHeaders({
      'Accept': 'text/plain',
      'Content-Type': 'application/json'
    });
    return this.http.post<string[]>(url, JSON.stringify(bearerToken), { headers })
      .pipe(
        map(res => res || []),
        catchError((err) => {
          console.error('Failed to fetch table attributes:', err);
          return throwError(() => err);
        })
      );
  }

  getObjectsFieldInfo(tableName: string): Observable<SObjectFieldInfo[]> {
    const bearerToken = sessionStorage.getItem('sf_access_token');
    this.validateBearerToken(bearerToken);
    const url = `${this.baseApiUrl}/GetTableColumnNames/${tableName}`;
    const headers = new HttpHeaders({
      'Accept': 'text/plain',
      'Content-Type': 'application/json'
    });
    return this.http.post<SObjectFieldInfo[]>(url, JSON.stringify(bearerToken), { headers })
      .pipe(
        map(res => res || []),
        catchError((err) => {
          console.error('Failed to fetch table fields info:', err);
          return throwError(() => err);
        })
      );
  }

  getProductData(rowCount: number): Observable<SalesforceProductRecord[]> {
    const bearerToken = sessionStorage.getItem('sf_access_token');
    this.validateBearerToken(bearerToken);
    const url = `${this.baseApiUrl}/CareProgramEnrolleeProduct/${rowCount}`;
    const headers = new HttpHeaders({
      'Accept': 'text/plain',
      'Content-Type': 'application/json'
    });
    return this.http.post<SalesforceProductRecord[]>(url, JSON.stringify(bearerToken), { headers })
      .pipe(
        map(res => res || []),
        catchError((err) => {
          console.error('Failed to fetch SalesforceProductRecords:', err);
          return throwError(() => err);
        })
      );
  }
}
