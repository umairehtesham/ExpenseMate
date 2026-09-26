import { Component, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ExpenseService } from '../../core/services/expense.service';
import { Expense } from '../../core/models/expense.model';

@Component({
  selector: 'app-expense',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="grid-2">
      <!-- Add New Expense Card -->
      <div class="card">
        <h2 class="card-title">💳 Add New Expense</h2>
        <form (ngSubmit)="addExpense()">
          <div class="form-group">
            <label>Category</label>
            <input 
              type="text" 
              [(ngModel)]="newCategory" 
              name="expenseCategory" 
              class="form-control" 
              placeholder="e.g. Food, Utilities, Rent" 
              required 
            />
          </div>
          <div class="form-group">
            <label>Amount ($)</label>
            <input 
              type="number" 
              [(ngModel)]="newAmount" 
              name="expenseAmount" 
              class="form-control" 
              placeholder="0.00" 
              min="1" 
              required 
            />
          </div>
          <button type="submit" class="btn btn-primary" style="width: 100%; margin-top: 0.5rem;">
            + Add Expense
          </button>
        </form>
      </div>

      <!-- Search Expense Record Card -->
      <div class="card">
        <h2 class="card-title">🔍 Search Expense Record</h2>
        <div class="form-group">
          <label>Expense ID</label>
          <div style="display: flex; gap: 0.5rem;">
            <input 
              type="text" 
              [(ngModel)]="searchId" 
              name="searchId" 
              class="form-control" 
              placeholder="Enter Expense ID..." 
              (keyup.enter)="searchById()"
            />
            <button type="button" class="btn btn-primary" (click)="searchById()">Search</button>
          </div>
        </div>

        <!-- Search Result Preview -->
        <div *ngIf="searchedExpense" style="margin-top: 1rem; padding: 1rem; background: #0f172a; border-radius: 8px; border: 1px solid var(--border-color);">
          <div style="display: flex; justify-content: space-between; align-items: center;">
            <span class="badge" style="background: rgba(239, 68, 68, 0.2); color: #f87171;">
              Found #{{ getExpenseId(searchedExpense) }}
            </span>
            <span style="font-weight: 700; color: #f87171; font-size: 1.2rem;">
              -\${{ getAmount(searchedExpense) | number:'1.2-2' }}
            </span>
          </div>
          
          <div style="margin-top: 0.75rem;">
            <p style="font-weight: 600; color: var(--text-main); margin: 0;">
              Category: <span style="color: #f87171;">{{ getCategory(searchedExpense) }}</span>
            </p>
            <p style="font-size: 0.85rem; color: var(--text-muted); margin: 0.25rem 0 0 0;">
              📅 Created At: {{ (getCreatedAt(searchedExpense) | date:'MMM d, y, h:mm a') || 'N/A' }}
            </p>
          </div>

          <div style="display: flex; justify-content: flex-end; margin-top: 0.75rem;">
            <button 
              type="button" 
              class="btn btn-danger" 
              style="padding: 0.4rem 0.8rem; font-size: 0.8rem;" 
              (click)="deleteExpense(getExpenseId(searchedExpense))"
            >
              🗑️ Delete
            </button>
          </div>
        </div>

        <div *ngIf="searchError" style="margin-top: 1rem; color: var(--danger); font-size: 0.9rem;">
          ⚠️ {{ searchError }}
        </div>
      </div>
    </div>

    <!-- Expense Records Table Card -->
    <div class="card">
      <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 1rem;">
        <h2 class="card-title" style="margin-bottom: 0;">📋 Expense Records</h2>
        <button type="button" class="btn btn-outline" (click)="fetchAllExpenses()">🔄 Fetch All Expenses</button>
      </div>

      <div *ngIf="expenses.length > 0; else emptyState" class="table-container">
        <table class="custom-table">
          <thead>
            <tr>
              <th>ID</th>
              <th>CATEGORY</th>
              <th>AMOUNT</th>
              <th>DATE & TIME</th>
              <th style="text-align: right;">ACTION</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let item of expenses">
              <td style="color: var(--text-muted);">#{{ getExpenseId(item) }}</td>
              <td>
                <span class="badge" style="background: rgba(239, 68, 68, 0.15); color: #f87171;">
                  {{ getCategory(item) }}
                </span>
              </td>
              <td style="color: #f87171; font-weight: 700;">-\${{ getAmount(item) | number:'1.2-2' }}</td>
              <td style="color: var(--text-muted); font-size: 0.9rem;">
                {{ (getCreatedAt(item) | date:'MMM d, y, h:mm a') || 'N/A' }}
              </td>
              <td style="text-align: right;">
                <button 
                  type="button" 
                  class="btn btn-danger" 
                  style="padding: 0.35rem 0.75rem; font-size: 0.8rem;" 
                  (click)="deleteExpense(getExpenseId(item))"
                >
                  🗑️ Delete
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <ng-template #emptyState>
        <div style="text-align: center; padding: 2rem; color: var(--text-muted);">
          No expense records loaded yet. Click <strong>"Fetch All Expenses"</strong> above.
        </div>
      </ng-template>
    </div>
  `
})
export class ExpenseComponent {
  expenses: any[] = [];
  
  newCategory: string = '';
  newAmount: number | null = null;

  searchId: string = '';
  searchedExpense: any = null;
  searchError: string = '';

  constructor(
    private expenseService: ExpenseService,
    private cdr: ChangeDetectorRef
  ) {}

  fetchAllExpenses(): void {
    this.expenseService.getAllExpenses().subscribe({
      next: (data: any) => {
        this.expenses = Array.isArray(data) ? data : [];
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error fetching expenses:', err);
        this.cdr.detectChanges();
      }
    });
  }

  addExpense(): void {
    if (!this.newCategory || !this.newAmount) return;

    const categoryVal = this.newCategory.trim();
    const amountVal = Number(this.newAmount);
    const createdDateVal = new Date().toISOString();

    // Directly maps to public fields in C# Expense domain class
    const payload = {
      expenseCategory: categoryVal,
      ExpenseCategory: categoryVal,
      expenseAmount: amountVal,
      ExpenseAmount: amountVal,
      createdAt: createdDateVal,
      CreatedAt: createdDateVal
    };

    this.expenseService.addExpense(payload as any).subscribe({
      next: () => {
        this.newCategory = '';
        this.newAmount = null;
        this.fetchAllExpenses();
      },
      error: (err) => {
        console.error('Error adding expense:', err);
        this.cdr.detectChanges();
      }
    });
  }

  searchById(): void {
    const term = this.searchId ? this.searchId.toString().trim() : '';
    if (!term) {
      this.searchError = 'Please enter a valid Expense ID.';
      this.searchedExpense = null;
      this.cdr.detectChanges();
      return;
    }

    this.searchError = '';
    this.searchedExpense = null;

    this.expenseService.getExpenseById(term).subscribe({
      next: (data: any) => {
        const result = Array.isArray(data) ? data[0] : data;

        if (result && this.getExpenseId(result) !== 'N/A') {
          this.searchedExpense = result;
          this.searchError = '';
        } else {
          this.searchedExpense = null;
          this.searchError = `No expense record found with ID "${term}".`;
        }
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Search error:', err);
        this.searchedExpense = null;
        this.searchError = `No expense record found with ID "${term}".`;
        this.cdr.detectChanges();
      }
    });
  }

  deleteExpense(id: string | number): void {
    if (id === 'N/A' || id === undefined || id === null) return;

    this.expenseService.deleteExpense(id).subscribe({
      next: () => {
        if (this.searchedExpense && String(this.getExpenseId(this.searchedExpense)) === String(id)) {
          this.searchedExpense = null;
        }
        this.fetchAllExpenses();
      },
      error: (err) => {
        console.error('Error deleting expense:', err);
        this.cdr.detectChanges();
      }
    });
  }

  // Value Resolvers matching C# Expense properties: ExpenseId, ExpenseCategory, ExpenseAmount, CreatedAt
  getExpenseId(item: any): string | number {
    if (!item) return 'N/A';
    return item.expenseId ?? item.ExpenseId ?? item.id ?? item.Id ?? 'N/A';
  }

  getCategory(item: any): string {
    if (!item) return 'Uncategorized';
    return item.expenseCategory ?? item.ExpenseCategory ?? item.category ?? item.Category ?? 'Uncategorized';
  }

  getAmount(item: any): number {
    if (!item) return 0;
    const val = item.expenseAmount ?? item.ExpenseAmount ?? item.amount ?? item.Amount;
    return val !== undefined && val !== null ? Number(val) : 0;
  }

  getCreatedAt(item: any): any {
    if (!item) return null;
    return item.createdAt ?? item.CreatedAt ?? item.expenseDate ?? item.ExpenseDate ?? null;
  }
}