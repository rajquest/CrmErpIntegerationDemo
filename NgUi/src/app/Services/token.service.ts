import { Injectable } from '@angular/core';
import { HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { BaseService } from './base.service';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class TokenService extends BaseService {

  private inforErpBaseUrl = `${environment.apiBaseUrl}/InforAuth`;
  private salesforceBaseUrl = `${environment.apiBaseUrl}/Salesforce`;

  getToken(): Observable<string> {
    console.log("Infor Token Request");
    return this.http.get(`${this.inforErpBaseUrl}/GetToken`, {
      responseType: 'text'
    });
  }

  getApiToken(bearerToken: string): Observable<string> {
    return this.http.post(`${this.inforErpBaseUrl}/GetApiToken`, JSON.stringify(bearerToken), {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      }),
      responseType: 'text'
    });
  }

  getSalesforceApiToken(): Observable<string> {
    return this.http.get(`${this.salesforceBaseUrl}/GetToken`, {
      responseType: 'text'
    });
  }

  checkInforTokensAndPrompt(): boolean {
    const storedToken = sessionStorage.getItem('infor_access_token');
    const apiStoredToken = sessionStorage.getItem('infor_api_access_token');
    const overrideToken = sessionStorage.getItem('inforAPIOverideToken');
    if (!overrideToken && (!storedToken || !apiStoredToken)) {
      console.warn('Missing Infor tokens. Generate or refresh new tokens, or set an override token.');
      return false;
    }
    return true;
  }

  checkSalesforceTokensAndPrompt(): boolean {
    const storedToken = sessionStorage.getItem('sf_access_token');
    if (!storedToken) {
      console.warn('Missing salesforce tokens.');
      return false;
    }
    return true;
  }

  getInforApiToken(): string {
    const overrideToken = sessionStorage.getItem('inforAPIOverideToken');
    if (overrideToken) return overrideToken;
    const token = sessionStorage.getItem('infor_api_access_token');
    if (!token) {
      throw new Error('Missing Infor API token.');
    }
    return token;
  }

  setInforApiToken(token: string): void {
    sessionStorage.setItem('infor_api_access_token', token);
  }

  getInforApiOverrideToken(): string | null {
    return sessionStorage.getItem('inforAPIOverideToken');
  }
}
