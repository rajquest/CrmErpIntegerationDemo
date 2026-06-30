import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TokenService } from '../../Services/token.service';

@Component({
  selector: 'app-get-token',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './get-token.component.html',
  styleUrl: './get-token.component.scss'
})
export class GetTokenComponent implements OnInit {
  accessToken = signal('');
  errorMessage = signal('');
  inforTokenFetched = signal(false);

  apiAccessToken = signal('');
  appErrorMessage = signal('');
  inforApiTokenFetched = signal(false);

  sfAccessToken = signal('');
  sfErrorMessage = signal('');
  sfTokenFetched = signal(false);

  overrideTokenInput = '';
  overrideSaved = signal(false);

  constructor(private tokenService: TokenService) { }

  ngOnInit(): void {
    const storedToken = sessionStorage.getItem('infor_access_token');
    if (storedToken) {
      this.accessToken.set(storedToken);
      this.inforTokenFetched.set(true);
    }
    const apiStoredToken = sessionStorage.getItem('infor_api_access_token');
    if (apiStoredToken) {
      this.apiAccessToken.set(apiStoredToken);
      this.inforApiTokenFetched.set(true);
    }
    const sfStoredToken = sessionStorage.getItem('sf_access_token');
    if (sfStoredToken) {
      this.sfAccessToken.set(sfStoredToken);
      this.sfTokenFetched.set(true);
    }
    const overrideToken = sessionStorage.getItem('inforAPIOverideToken');
    if (overrideToken) {
      this.overrideTokenInput = overrideToken;
      this.overrideSaved.set(true);
    }
  }

  getInforAppToken() {
    this.errorMessage.set('');
    this.accessToken.set('');
    this.inforTokenFetched.set(true);

    this.tokenService.getToken().subscribe({
      next: (token) => {
        this.accessToken.set(token);
        sessionStorage.setItem('infor_access_token', token);
      },
      error: (err) => {
        this.errorMessage.set(err.message || 'Error fetching token');
        console.error('Token fetch error:', err);
      }
    });
  }

  getInforApiToken() {
    this.appErrorMessage.set('');
    this.apiAccessToken.set('');
    this.inforApiTokenFetched.set(true);

    const access_token = sessionStorage.getItem('infor_access_token') ?? '';
    if (!access_token.trim()) {
      this.appErrorMessage.set('Infor App OAuth token is required first.');
      return;
    }

    this.tokenService.getApiToken(access_token).subscribe({
      next: (token) => {
        if (token) {
          this.apiAccessToken.set(token);
          sessionStorage.setItem('infor_api_access_token', token);
        } else {
          this.appErrorMessage.set('Invalid response format from API.');
        }
      },
      error: (err) => {
        this.appErrorMessage.set(err.message || 'App token request failed');
        console.error('Token request failed:', err);
      }
    });
  }

  getSalesforceToken() {
    this.sfErrorMessage.set('');
    this.sfAccessToken.set('');
    this.sfTokenFetched.set(true);

    this.tokenService.getSalesforceApiToken().subscribe({
      next: (token) => {
        this.sfAccessToken.set(token);
        sessionStorage.setItem('sf_access_token', token);
      },
      error: (err) => {
        this.sfErrorMessage.set(err.message || 'Error fetching token');
        console.error('Token fetch error:', err);
      }
    });
  }

  saveOverrideToken() {
    const token = this.overrideTokenInput.trim();
    if (!token) return;
    sessionStorage.setItem('inforAPIOverideToken', token);
    this.overrideSaved.set(true);
  }

  clearTokens() {
    sessionStorage.removeItem('infor_access_token');
    sessionStorage.removeItem('infor_api_access_token');
    sessionStorage.removeItem('sf_access_token');
    sessionStorage.removeItem('inforAPIOverideToken');

    this.accessToken.set('');
    this.errorMessage.set('');
    this.inforTokenFetched.set(false);

    this.apiAccessToken.set('');
    this.appErrorMessage.set('');
    this.inforApiTokenFetched.set(false);

    this.sfAccessToken.set('');
    this.sfErrorMessage.set('');
    this.sfTokenFetched.set(false);

    this.overrideTokenInput = '';
    this.overrideSaved.set(false);
  }
}
