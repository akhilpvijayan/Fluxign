import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})

export class EmailService {
  constructor(
    private http: HttpClient,
    @Inject('API_BASE_URL') private baseUrl: string
  ) {}

  sendEmail(payload: any): Observable<any> {
  
    return this.http.post(`${this.baseUrl}/Signature/initiate`, payload);
  }
}