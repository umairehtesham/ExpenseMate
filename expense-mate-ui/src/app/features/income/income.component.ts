import { Component, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { IncomeService } from '../../core/services/income.service';
import { Income } from '../../core/models/income.model';

@Component({
  selector: 'app-income',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="grid-2">
      <!-- Add New Income Card -->
      <div class="card">
        <h2 class="card-title">💵 Add New Income</h2>
        <form (ngSubmit)="addIncome()">
          <div class="form-group">
            <label>Category</label>
            <input 
              type="text" 
              [(ngModel)]="newCategory" 
              name="incomeCategory" 
              class="form-control" 
              placeholder="e.g. Primary Job, Freelance" 
              required 
            />
          </div>
          <div class="form-group">
            <label>Amount ($)</label>
            <input 
              type="number" 
              [(ngModel)]="newAmount" 
              name="incomeAmount" 
              class="form-control" 
              placeholder="0" 
              min="1" 
              required 
            />
          </div>
          <button type="submit" class="btn btn-success" style="width: 100%; margin-top: 0.5rem; background-color: #10b981; border: none; color: white; padding: 0.6rem; border-radius: 6px; font-weight: 600; cursor: pointer;">
            + Add Income
          </button>
        </form>
      </div>

      <!-- Search Income Record Card -->
      <div class="card">
        <h2 class="card-title">🔍 Search Income Record</h2>
        <div class="form-group">
          <label>Income ID</label>
          <div style="display: flex; gap: 0.5rem;">
            <input 
              type="text" 
              [(ngModel)]="searchId" 
              name="searchId" 
              class="form-control" 
              placeholder="Enter Income ID..." 
              (keyup.enter)="searchById()"
            />
            <button type="button" class="btn btn-primary" (click)="searchById()">Search</button>
          </div>
        </div>

        <!-- Search Result Preview -->
        <div *ngIf="searchedIncome" style="margin-top: 1rem; padding: 1rem; background: #0f172a; border-radius: 8px; border: 1px solid var(--border-color);">
          <div style="display: flex; justify-content: space-between; align-items: center;">
            <span class="badge" style="background: rgba(16, 185, 129, 0.2); color: #34d399;">
              Found #{{ getIncomeId(searchedIncome) }}
            </span>
            <span style="font-weight: 700; color: #34d399; font-size: 1.2rem;">
              +\${{ getAmount(searchedIncome) | number:'1.2-2' }}
            </span>
          </div>
          
          <div style="margin-top: 0.75rem;">
            <p style="font-weight: 600; color: var(--text-main); margin: 0;">
              Category: <span style="color: #34d399;">{{ getCategory(searchedIncome) }}</span>
            </p>
            <p style="font-size: 0.85rem; color: var(--text-muted); margin: 0.25rem 0 0 0;">
              📅 Created At: {{ (getCreatedAt(searchedIncome) | date:'MMM d, y, h:mm a') || 'N/A' }}
            </p>
          </div>

          <div style="display: flex; justify-content: flex-end; margin-top: 0.75rem;">
            <button 
              type="button" 
              class="btn btn-danger" 
              style="padding: 0.4rem 0.8rem; font-size: 0.8rem;" 
              (click)="deleteIncome(getIncomeId(searchedIncome))"
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

    <!-- Income Records Table Card -->
    <div class="card">
      <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 1rem;">
        <h2 class="card-title" style="margin-bottom: 0;">📋 Income Records</h2>
        <button type="button" class="btn btn-outline" (click)="fetchAllIncomes()">🔄 Fetch All Incomes</button>
      </div>

      <div *ngIf="incomes.length > 0; else emptyState" class="table-container">
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
            <tr *ngFor="let item of incomes">
              <td style="color: var(--text-muted);">#{{ getIncomeId(item) }}</td>
              <td>
                <span class="badge" style="background: rgba(16, 185, 129, 0.15); color: #34d399; padding: 0.25rem 0.6rem; border-radius: 12px; font-size: 0.85rem;">
                  {{ getCategory(item) }}
                </span>
              </td>
              <td style="color: #34d399; font-weight: 700;">+\${{ getAmount(item) | number:'1.2-2' }}</td>
              <td style="color: var(--text-muted); font-size: 0.9rem;">
                {{ (getCreatedAt(item) | date:'MMM d, y, h:mm a') || 'N/A' }}
              </td>
              <td style="text-align: right;">
                <button 
                  type="button" 
                  class="btn btn-danger" 
                  style="padding: 0.35rem 0.75rem; font-size: 0.8rem;" 
                  (click)="deleteIncome(getIncomeId(item))"
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
          No income records loaded yet. Click <strong>"Fetch All Incomes"</strong> above.
        </div>
      </ng-template>
    </div>
  `
})
export class IncomeComponent {
  incomes: any[] = [];
  
  newCategory: string = '';
  newAmount: number | null = null;

  searchId: string = '';
  searchedIncome: any = null;
  searchError: string = '';

  constructor(
    private incomeService: IncomeService,
    private cdr: ChangeDetectorRef
  ) {}

  fetchAllIncomes(): void {
    this.incomeService.getAllIncomes().subscribe({
      next: (data: any) => {
        this.incomes = Array.isArray(data) ? data : [];
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error fetching incomes:', err);
        this.cdr.detectChanges();
      }
    });
  }

  addIncome(): void {
    if (!this.newCategory || !this.newAmount) return;

    const categoryVal = this.newCategory.trim();
    const amountVal = Number(this.newAmount);
    const createdDateVal = new Date().toISOString();

    const payload = {
      incomeCategory: categoryVal,
      IncomeCategory: categoryVal,
      category: categoryVal,
      Category: categoryVal,
      incomeAmount: amountVal,
      IncomeAmount: amountVal,
      amount: amountVal,
      Amount: amountVal,
      createdAt: createdDateVal,
      CreatedAt: createdDateVal,
      incomeDate: createdDateVal,
      IncomeDate: createdDateVal
    };

    this.incomeService.addIncome(payload as any).subscribe({
      next: () => {
        this.newCategory = '';
        this.newAmount = null;
        this.fetchAllIncomes();
      },
      error: (err) => {
        console.error('Error adding income:', err);
        this.cdr.detectChanges();
      }
    });
  }

  searchById(): void {
    const term = this.searchId ? this.searchId.toString().trim() : '';
    if (!term) {
      this.searchError = 'Please enter a valid Income ID.';
      this.searchedIncome = null;
      this.cdr.detectChanges();
      return;
    }

    this.searchError = '';
    this.searchedIncome = null;

    this.incomeService.getIncomeById(term).subscribe({
      next: (data: any) => {
        const result = Array.isArray(data) ? data[0] : data;

        if (result && this.getIncomeId(result) !== 'N/A') {
          this.searchedIncome = result;
          this.searchError = '';
        } else {
          this.searchedIncome = null;
          this.searchError = `No income record found with ID "${term}".`;
        }
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Search error:', err);
        this.searchedIncome = null;
        this.searchError = `No income record found with ID "${term}".`;
        this.cdr.detectChanges();
      }
    });
  }

  deleteIncome(id: string | number): void {
    if (id === 'N/A' || id === undefined || id === null) return;

    this.incomeService.deleteIncome(id).subscribe({
      next: () => {
        if (this.searchedIncome && String(this.getIncomeId(this.searchedIncome)) === String(id)) {
          this.searchedIncome = null;
        }
        this.fetchAllIncomes();
      },
      error: (err) => {
        console.error('Error deleting income:', err);
        this.cdr.detectChanges();
      }
    });
  }

  // Value Resolvers matching C# entity properties
  getIncomeId(item: any): string | number {
    if (!item) return 'N/A';
    return item.incomeId ?? item.IncomeId ?? item.id ?? item.Id ?? 'N/A';
  }

  getCategory(item: any): string {
    if (!item) return 'Uncategorized';
    return item.incomeCategory ?? item.IncomeCategory ?? item.category ?? item.Category ?? 'Uncategorized';
  }

  getAmount(item: any): number {
    if (!item) return 0;
    const val = item.incomeAmount ?? item.IncomeAmount ?? item.amount ?? item.Amount;
    return val !== undefined && val !== null ? Number(val) : 0;
  }

  getCreatedAt(item: any): any {
    if (!item) return null;
    return item.createdAt ?? item.CreatedAt ?? item.incomeDate ?? item.IncomeDate ?? item.date ?? item.Date ?? null;
  }
}