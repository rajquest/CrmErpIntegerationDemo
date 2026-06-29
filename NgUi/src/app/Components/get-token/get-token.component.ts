import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TokenService } from '../../Services/token.service';

@Component({
  selector: 'app-get-token',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './get-token.component.html',
  styleUrl: './get-token.component.scss'
})
export class GetTokenComponent implements OnInit {
  loading = false;
  accessToken = '';
  errorMessage = '';
  apiAccessToken = '';
  appErrorMessage = '';
  sfAccessToken = '';
  sfErrorMessage = '';

  constructor(private tokenService: TokenService) { }

  ngOnInit(): void {
    const storedToken = sessionStorage.getItem('infor_access_token');
    if (storedToken) {
      this.accessToken = storedToken;
    }
    const apiStoredToken = sessionStorage.getItem('infor_api_access_token');
    if (apiStoredToken) {
      this.apiAccessToken = apiStoredToken;
    }
    const sfStoredToken = sessionStorage.getItem('sf_access_token');
    if (sfStoredToken) {
      this.sfAccessToken = sfStoredToken;
    }
  }

  getInforApiToken() {
    this.appErrorMessage = '';
    this.apiAccessToken = '';

    let access_token = sessionStorage.getItem('infor_access_token') ?? '';

    if (!access_token.trim()) {
      console.error('Access token missing');
      return;
    }

    this.tokenService.getApiToken(access_token).subscribe({
      next: (token) => {
        if (token) {
          this.apiAccessToken = token;
          sessionStorage.setItem('infor_api_access_token', this.apiAccessToken);
        } else {
          this.appErrorMessage = 'Invalid response format from API.';
          console.warn('Unexpected API response:', token);
        }
      },
      error: (err) => {
        this.appErrorMessage = err.message || 'App token request failed';
        console.error('Token request failed:', err);
      }
    });
  }

  getInforAppToken() {
    this.loading = true;
    this.errorMessage = '';
    this.accessToken = '';

    this.tokenService.getToken().subscribe({
      next: (token) => {
        this.accessToken = token;
        sessionStorage.setItem('infor_access_token', this.accessToken);
        this.loading = false;
      },
      error: (err) => {
        this.loading = false;
        this.errorMessage = err.message || 'Error fetching token';
        console.error('Token fetch error:', err);
      }
    });
  }

  getSalesforceToken() {
    this.loading = true;
    this.sfErrorMessage = '';
    this.sfAccessToken = '';

    this.tokenService.getSalesforceApiToken().subscribe({
      next: (token) => {
        this.sfAccessToken = token;
        sessionStorage.setItem('sf_access_token', this.sfAccessToken);
        this.loading = false;
      },
      error: (err) => {
        this.loading = false;
        this.sfErrorMessage = err.message || 'Error fetching token';
        console.error('Token fetch error:', err);
      }
    });
  }
}
