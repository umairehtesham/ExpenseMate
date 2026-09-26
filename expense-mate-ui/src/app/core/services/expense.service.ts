import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Expense } from '../models/expense.model';

@Injectable({ providedIn: 'root' })
export class ExpenseService {
  private apiUrl = 'http://localhost:5001/api/expense';

  constructor(private http: HttpClient) {}

  // GetAllExpensesAsync()
  getAllExpenses(): Observable<Expense[]> {
    return this.http.get<Expense[]>(this.apiUrl);
  }

  // GetExpenseByIdAsync(int id)
  getExpenseById(id: string | number): Observable<Expense> {
    return this.http.get<Expense>(`${this.apiUrl}/${id}`);
  }

  // AddExpenseAsync(Expense expense)
  addExpense(expense: any): Observable<Expense> {
    return this.http.post<Expense>(this.apiUrl, expense);
  }

  // UpdateExpenseAsync(Expense expense)
  updateExpense(id: string | number, expense: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, expense);
  }

  // DeleteExpenseAsync(int id)
  deleteExpense(id: string | number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }

  // GetTotalSpentByMonthAsync(string month)
  getTotalSpentByMonth(month: string): Observable<number> {
    return this.http.get<number>(`${this.apiUrl}/total-spent/${month}`);
  }
}