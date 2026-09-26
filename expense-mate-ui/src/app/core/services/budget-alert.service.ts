import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class BudgetAlertService {
  private baseUrl = 'http://localhost:5001/api/budgetalert';

  constructor(private http: HttpClient) {}

  getBudgetAlert(month: string): Observable<any> {
    const encodedMonth = encodeURIComponent(month);
    return this.http.get<any>(`${this.baseUrl}/${encodedMonth}`);
  }
}