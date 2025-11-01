import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AuthService {
  constructor(@Inject('API_BASE_URL') private baseUrl: string, private http: HttpClient, private router: Router) {}
  private loggedIn = new BehaviorSubject<boolean>(!!localStorage.getItem('authToken'));

  get isLoggedIn() {
    return this.loggedIn.asObservable();
  }

  login(token: string) {
    localStorage.setItem('authToken', token);
    this.loggedIn.next(true);
  }

  logout() {
    localStorage.removeItem('authToken');
    this.loggedIn.next(false);
  }

  getAccessToken() {
    return localStorage.getItem('authToken');
  }

  getRefreshToken() {
    return localStorage.getItem('refreshToken');
  }

  storeTokens(access: string, refresh: string) {
    localStorage.setItem('authToken', access);
    localStorage.setItem('refreshToken', refresh);
  }

  refreshToken(refreshToken: any): Observable<any> {
    return this.http.post(`${this.baseUrl}/users/auth/refresh-token`, refreshToken);
  }

  confirmPassword(password: string) {
    return this.http.get(`${this.baseUrl}/users/auth/confirmpassword/${encodeURIComponent(password)}`);
  }  

  resetPassword(token: string, newPassword: string): Observable<any> {
    return this.http.post(`${this.baseUrl}/users/auth/reset-password`, {
      token,
      newPassword
    });
  }
}
