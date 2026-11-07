import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BudgetService } from '../../core/services/budget.service';
import { BudgetAlertService } from '../../core/services/budget-alert.service';

interface BudgetAlertResult {
  isExceeded: boolean;
  totalSpent: number;
  budgetAmount: number;
  percentUsed: number;
  alertPercent: number;
  message: string;
}

@Component({
  selector: 'app-budget',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="grid-2">
      <!-- Card 1: Check Monthly Budget Alert -->
      <div class="card">
        <h2 class="card-title">🚨 Check Monthly Budget Alert</h2>

        <div class="form-group">
          <label>Target Month</label>
          <div style="display: flex; gap: 0.5rem; margin-top: 0.35rem;">
            <select [(ngModel)]="checkMonth" name="checkMonth" class="form-control" style="flex: 1;">
              <option *ngFor="let m of months" [value]="m">{{ m }}</option>
            </select>
            <button type="button" class="btn btn-primary" (click)="checkStatus()">
              Check Status
            </button>
          </div>
        </div>

        <!-- Alert Display Panel -->
        <div *ngIf="alertResult" style="margin-top: 1.25rem; padding: 1rem; background: #0f172a; border-radius: 8px; border: 1px solid var(--border-color, #1e293b);">

          <!-- Header Status & Totals -->
          <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 0.75rem;">
            <span class="badge"
                  [style.background]="alertResult.isExceeded ? 'rgba(239, 68, 68, 0.2)' : 'rgba(16, 185, 129, 0.2)'"
                  [style.color]="alertResult.isExceeded ? '#f87171' : '#34d399'"
                  style="font-weight: 600; padding: 0.35rem 0.65rem; border-radius: 4px; font-size: 0.85rem;">
              {{ alertResult.isExceeded ? '⚠️ Alert Triggered' : '✅ Within Budget' }}
            </span>
            <span style="font-weight: 700; color: #ffffff; font-size: 0.95rem;">
              Spent: \${{ alertResult.totalSpent | number:'1.2-2' }} / \${{ alertResult.budgetAmount | number:'1.2-2' }}
            </span>
          </div>

          <!-- Progress Bar -->
          <div style="background: #1e293b; border-radius: 6px; height: 10px; width: 100%; overflow: hidden; margin: 0.75rem 0;">
            <div [style.width.%]="alertResult.percentUsed > 100 ? 100 : alertResult.percentUsed"
                 [style.background]="alertResult.isExceeded ? '#f87171' : '#34d399'"
                 style="height: 100%; transition: width 0.4s ease-in-out;">
            </div>
          </div>

          <!-- Usage Metrics -->
          <div style="display: flex; justify-content: space-between; font-size: 0.85rem; color: #94a3b8; margin-top: 0.5rem;">
            <span>Used: <strong>{{ alertResult.percentUsed | number:'1.0-1' }}%</strong></span>
            <span>Alert Threshold: <strong>{{ alertResult.alertPercent }}%</strong></span>
          </div>

          <!-- Backend Alert Message -->
          <div *ngIf="alertResult.message" style="margin-top: 0.75rem; font-size: 0.85rem; color: #cbd5e1; font-style: italic; border-top: 1px solid #1e293b; padding-top: 0.5rem;">
            "{{ alertResult.message }}"
          </div>
        </div>

        <!-- Error Message Display -->
        <div *ngIf="alertMessage" style="margin-top: 1rem; color: #f87171; font-size: 0.9rem; padding: 0.5rem; background: rgba(239, 68, 68, 0.1); border-radius: 6px;">
          {{ alertMessage }}
        </div>
      </div>

      <!-- Card 2: Set Monthly Budget -->
      <div class="card">
        <h2 class="card-title">🎯 Set Monthly Budget</h2>

        <!-- Error Alert Banner -->
        <div
          *ngIf="errorMessage"
          class="alert-danger"
          style="margin-bottom: 1rem; padding: 0.75rem 1rem; background: rgba(239, 68, 68, 0.15); border: 1px solid #ef4444; color: #f87171; border-radius: 6px; font-size: 0.9rem;"
        >
          ⚠️ {{ errorMessage }}
        </div>

        <!-- Success Alert Banner -->
        <div
          *ngIf="statusMessage"
          class="alert-success"
          style="margin-bottom: 1rem; padding: 0.75rem 1rem; background: rgba(16, 185, 129, 0.15); border: 1px solid #10b981; color: #34d399; border-radius: 6px; font-size: 0.9rem;"
        >
          ✅ {{ statusMessage }}
        </div>

        <form (ngSubmit)="saveBudget()">
          <div class="form-group">
            <label>Month</label>
            <select [(ngModel)]="newBudgetMonth" name="newBudgetMonth" class="form-control" style="margin-top: 0.35rem;">
              <option *ngFor="let m of months" [value]="m">{{ m }}</option>
            </select>
          </div>

          <div class="form-group" style="margin-top: 0.85rem;">
            <label>Budget Amount ($)</label>
            <input
              type="number"
              [(ngModel)]="newBudgetAmount"
              name="newBudgetAmount"
              class="form-control"
              placeholder="1000.00"
              style="margin-top: 0.35rem;"
            />
          </div>

          <div class="form-group" style="margin-top: 0.85rem;">
            <label>Alert Threshold (%)</label>
            <input
              type="number"
              [(ngModel)]="newAlertPercent"
              name="newAlertPercent"
              class="form-control"
              placeholder="80"
              style="margin-top: 0.35rem;"
            />
          </div>

          <button type="submit" class="btn btn-success" style="width: 100%; margin-top: 1.25rem; background-color: #10b981; border: none; color: white; padding: 0.65rem; border-radius: 6px; font-weight: 600; cursor: pointer;">
            Save Budget
          </button>
        </form>
      </div>
    </div>
  `
})
export class BudgetComponent implements OnInit {
  months: string[] = [
    'January', 'February', 'March', 'April', 'May', 'June',
    'July', 'August', 'September', 'October', 'November', 'December'
  ];

  checkMonth: string = 'September';
  alertResult: BudgetAlertResult | null = null;
  alertMessage: string = '';

  newBudgetMonth: string = 'September';
  newBudgetAmount: number | null = 100;
  newAlertPercent: number = 80;
  statusMessage: string = '';
  errorMessage: string = '';

  constructor(
    private budgetService: BudgetService,
    private budgetAlertService: BudgetAlertService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.checkStatus();
  }

  checkStatus(): void {
    if (!this.checkMonth) return;

    this.alertMessage = '';
    this.alertResult = null;

    this.budgetAlertService.getBudgetAlert(this.checkMonth).subscribe({
      next: (res: any) => {
        if (res) {
          // Robust mapping supporting both camelCase and PascalCase properties
          const isTriggered = res.isAlertTriggered ?? res.IsAlertTriggered ?? false;
          const totalSpent = res.totalSpent ?? res.TotalSpent ?? 0;
          const budgetLimit = res.budgetLimit ?? res.BudgetLimit ?? res.budgetAmount ?? res.BudgetAmount ?? 0;
          const spentPercentage = res.spentPercentage ?? res.SpentPercentage ?? res.percentUsed ?? res.PercentUsed ?? 0;
          const alertPercent = res.alertPercent ?? res.AlertPercent ?? res.alertThreshold ?? res.AlertThreshold ?? this.newAlertPercent ?? 80;
          const msg = res.message ?? res.Message ?? '';

          this.alertResult = {
            isExceeded: isTriggered,
            totalSpent: totalSpent,
            budgetAmount: budgetLimit,
            percentUsed: spentPercentage,
            alertPercent: alertPercent,
            message: msg
          };
        } else {
          this.alertMessage = `No budget alert configured for ${this.checkMonth}.`;
        }
        this.cdr.detectChanges();
      },
      error: (err: any) => {
        console.error('Fetch budget alert error:', err);
        this.alertMessage = err.error?.message || `Unable to fetch alert status for ${this.checkMonth}.`;
        this.cdr.detectChanges();
      }
    });
  }

  saveBudget(): void {
    this.errorMessage = '';
    this.statusMessage = '';

    // 1. Missing Required Fields Validation
    if (!this.newBudgetMonth || this.newBudgetAmount === null || this.newBudgetAmount === undefined) {
      this.errorMessage = 'Please provide both a Month and a Budget Amount.';
      this.cdr.detectChanges();
      return;
    }

    // 2. Negative / Zero Budget Amount Validation
    if (this.newBudgetAmount <= 0) {
      this.errorMessage = 'Budget amount must be greater than zero.';
      this.cdr.detectChanges();
      return;
    }

    // 3. Alert Threshold Range Validation (1% to 100%)
    if (this.newAlertPercent <= 0 || this.newAlertPercent > 100) {
      this.errorMessage = 'Alert threshold percentage must be between 1 and 100.';
      this.cdr.detectChanges();
      return;
    }

    // Payload supporting all potential DTO field name mappings
    const payload = {
      budgetMonth: this.newBudgetMonth,
      BudgetMonth: this.newBudgetMonth,
      month: this.newBudgetMonth,
      Month: this.newBudgetMonth,
      budgetAmount: Number(this.newBudgetAmount),
      BudgetAmount: Number(this.newBudgetAmount),
      alertPercent: Number(this.newAlertPercent || 80),
      AlertPercent: Number(this.newAlertPercent || 80),
      alertThreshold: Number(this.newAlertPercent || 80),
      AlertThreshold: Number(this.newAlertPercent || 80)
    };

    this.budgetService.saveBudget(payload as any).subscribe({
      next: (res: any) => {
        this.statusMessage = res?.message || `Budget for ${this.newBudgetMonth} saved successfully!`;
        setTimeout(() => (this.statusMessage = ''), 3500);

        // Automatically update alert view to match saved month
        this.checkMonth = this.newBudgetMonth;
        this.checkStatus();
      },
      error: (err: any) => {
        console.error('Save budget error:', err);
        this.errorMessage = err.error?.message || err.error || 'Failed to save budget record.';
        this.cdr.detectChanges();
      }
    });
  }
}
