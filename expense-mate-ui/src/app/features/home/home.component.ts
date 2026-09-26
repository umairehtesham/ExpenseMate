import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [RouterLink],
  template: `
    <div style="margin-bottom: 2rem;">
      <h1 style="font-size: 1.8rem; font-weight: 700;">Welcome to ExpenseMate</h1>
      <p style="color: var(--text-muted);">Your personal financial control center.</p>
    </div>

    <div class="grid-3">
      <div class="card">
        <h3 class="card-title">💵 Income Management</h3>
        <p style="color: var(--text-muted); font-size: 0.9rem; margin-bottom: 1.5rem;">
          Track earnings, view sources, and search individual records by ID.
        </p>
        <a routerLink="/income" class="btn btn-success" style="width: 100%;">Go to Income →</a>
      </div>

      <div class="card">
        <h3 class="card-title">💳 Expense Tracker</h3>
        <p style="color: var(--text-muted); font-size: 0.9rem; margin-bottom: 1.5rem;">
          Monitor day-to-day spending and organize expenses by category.
        </p>
        <a routerLink="/expenses" class="btn btn-primary" style="width: 100%;">Go to Expenses →</a>
      </div>

      <div class="card">
        <h3 class="card-title">🎯 Budget & Alerts</h3>
        <p style="color: var(--text-muted); font-size: 0.9rem; margin-bottom: 1.5rem;">
          Set monthly spending limits and check real-time threshold alerts.
        </p>
        <a routerLink="/budgets" class="btn btn-outline" style="width: 100%;">Go to Budgets →</a>
      </div>
    </div>
  `
})
export class HomeComponent {}