import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserVerifyService {

  constructor(
     private http: HttpClient,
     @Inject('API_BASE_URL') private baseUrl: string
   ) {}

   verifyUser(): Observable<any> {
      return this.http.get(`${this.baseUrl}/Signature/VerifyUser`);
    }
}
