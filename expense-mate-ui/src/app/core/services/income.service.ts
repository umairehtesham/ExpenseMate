import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Income } from '../models/income.model';

@Injectable({ providedIn: 'root' })
export class IncomeService {
  private apiUrl = 'http://localhost:5001/api/income';

  constructor(private http: HttpClient) {}

  getAllIncomes(): Observable<Income[]> {
    return this.http.get<Income[]>(this.apiUrl);
  }

  getIncomeById(id: string | number): Observable<Income> {
    return this.http.get<Income>(`${this.apiUrl}/${id}`);
  }

  addIncome(income: any): Observable<Income> {
    return this.http.post<Income>(this.apiUrl, income);
  }

  updateIncome(id: string | number, income: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, income);
  }

  deleteIncome(id: string | number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}