import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CsvImportResponse } from '../models/csv.model';

@Injectable({
  providedIn: 'root'
})
export class CsvService {
  private http = inject(HttpClient);
  private baseUrl = 'http://localhost:5001/api/Csv';

  importCsv(file: File): Observable<CsvImportResponse> {
    const formData = new FormData();
    formData.append('file', file, file.name);
    return this.http.post<CsvImportResponse>(`${this.baseUrl}/import`, formData);
  }

  exportCsv(type: string = 'all'): Observable<Blob> {
    return this.http.get(`${this.baseUrl}/export/${type}`, {
      responseType: 'blob'
    });
  }
}
