import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class BudgetService {
  private baseUrl = 'http://localhost:5001/api/budget';

  constructor(private http: HttpClient) {}

  // 🎯 Add this method to resolve the red line in budget.component.ts
  getBudgetByMonth(month: string): Observable<any> {
    const encodedMonth = encodeURIComponent(month);
    return this.http.get<any>(`${this.baseUrl}/${encodedMonth}`);
  }

  saveBudget(budget: any): Observable<any> {
    return this.http.post<any>(this.baseUrl, budget);
  }
}