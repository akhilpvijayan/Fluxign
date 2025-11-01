import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class TransactionDocService {

  constructor(
    private http: HttpClient,
    @Inject('API_BASE_URL') private baseUrl: string
  ) {}

    getFilePath(sessionId: string, processId: string): Observable<any> {
      return this.http.get<any>(`${this.baseUrl}/Signature/SignatureStatus/` + sessionId + '/' + processId);
    }

    getSignedDoc(sessionId: string): Observable<any> {
      return this.http.get<any>(`${this.baseUrl}/Signature/GetSignedDocument/` + sessionId);
    }
}
