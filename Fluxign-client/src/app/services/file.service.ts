import { Inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

interface FilePathResponse {
  filePath: string;
}

interface FileContentResponse {
  fileUrl: string;
}

@Injectable({
  providedIn: 'root',
})
export class FileService {
  constructor(
    private http: HttpClient,
    @Inject('API_BASE_URL') private baseUrl: string
  ) {}

  getFilePath(): Observable<FilePathResponse> {
    return this.http.get<FilePathResponse>(`${this.baseUrl}/getFilePath`);
  }

  getFileContent(path: string): Observable<FileContentResponse> {
    return this.http.post<FileContentResponse>(`${this.baseUrl}/getFile`, { path });
  }
  uploadFile(formData: FormData): Observable<any> {
    debugger
    return this.http.post(`${this.baseUrl}/FileUpload`, formData);
  }
  
}
