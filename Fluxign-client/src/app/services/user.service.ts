import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserService {

constructor(
    private http: HttpClient,
    @Inject('API_BASE_URL') private baseUrl: string
  ) {}

    getProfile(): Observable<any> {
      return this.http.get(`${this.baseUrl}/users/profile`);
    }

    updateProfile(userData: any): Observable<any> {
      return this.http.put(`${this.baseUrl}/users/updateuser`, userData);
    }

    requestPasswordReset(email: string): Observable<any> {
      return this.http.post(`${this.baseUrl}/users/request-password-reset`, { email });
    }    
}
