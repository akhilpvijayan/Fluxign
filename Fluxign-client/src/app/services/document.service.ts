import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class DocumentService {

  constructor(
    private http: HttpClient,
    @Inject('API_BASE_URL') private baseUrl: string
  ) { }

  GetOriginalDocumentByRecipientToken(token: string): Observable<any> {
    return this.http.get(`${this.baseUrl}/original-document/token?${token}`);
  }
}
