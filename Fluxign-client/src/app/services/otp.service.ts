import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class OtpService {

constructor(
    private http: HttpClient,
    @Inject('API_BASE_URL') private baseUrl: string
  ) {}

  requestOtp(purpose: string): Observable<any> {
    return this.http.post(`${this.baseUrl}/users/otp/request`, JSON.stringify(purpose), {
      headers: { 'Content-Type': 'application/json' }
    });
  }

  validateOtp(otpCode: string, purpose: string): Observable<any> {
    const body = {
      otpCode: otpCode,
      purpose: purpose
    };
  
    return this.http.post(`${this.baseUrl}/users/otp/validate`, body, {
      headers: { 'Content-Type': 'application/json' }
    });
  }  
}
