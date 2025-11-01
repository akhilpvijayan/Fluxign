import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import * as signalR from '@microsoft/signalr';
import { RequestDashboard } from '../models/request-dashboard';

@Injectable({
  providedIn: 'root'
})
export class RequestService {

  private hubConnection!: signalR.HubConnection;
  private requestsSubject = new BehaviorSubject<RequestDashboard[]>([]);
  requests$ = this.requestsSubject.asObservable();
  constructor(
    private http: HttpClient,
    @Inject('API_BASE_URL') private baseUrl: string
  ) { }

  getUserRequests(): Observable<any> {
    return this.http.get(`${this.baseUrl}/signingrequest`);
  }

  getRequestById(id: string): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/signingrequest/${id}`);
  }
  
  createSignRequest(payload: any): Observable<any> {
    return this.http.post(`${this.baseUrl}/signingrequest`, payload);
  }

  updateSignRequest(payload: any): Observable<any> {
    return this.http.put(`${this.baseUrl}/signingrequest`, payload);
  }

  getDashboard(params: {
    statusFilter?: string;
    nameFilter?: string;
    pageNumber?: number;
    pageSize?: number;
  }) {
    let httpParams = new HttpParams();
    if (params.statusFilter) httpParams = httpParams.set('statusFilter', params.statusFilter);
    if (params.nameFilter) httpParams = httpParams.set('nameFilter', params.nameFilter);
    httpParams = httpParams.set('pageNumber', (params.pageNumber ?? 1).toString());
    httpParams = httpParams.set('pageSize', (params.pageSize ?? 10).toString());

    this.http.get<RequestDashboard[]>(`${this.baseUrl}/signingrequest/dashboard`, { params: httpParams })
      .subscribe(
        (data) => this.requestsSubject.next(data),
        (error) => console.error('Failed to load dashboard', error)
      );
  }

  stopConnection() {
    if (this.hubConnection && this.hubConnection.state !== signalR.HubConnectionState.Disconnected) {
      this.hubConnection.stop().then(() => {
        console.log('SignalR connection stopped.');
      }).catch(err => console.error('Error stopping SignalR:', err));
    }
  }  
  
  private startSignalRConnection() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${this.baseUrl}/hubs/status`)
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('SignalR connected'))
      .catch((err) => console.error('SignalR connection error:', err));

    this.hubConnection.on('StatusUpdated', (update: any) => {
      console.log('Status update received', update);
      this.updateRequestStatus(update.requestId, update.newStatus);
    });
  }

  private updateRequestStatus(requestId: string, newStatus: string) {
    const current = this.requestsSubject.value;
    const updated = current.map((req) => {
      if (req.requestId === requestId) {
        return { ...req, status: newStatus };
      }
      return req;
    });
    this.requestsSubject.next(updated);
  }
}
