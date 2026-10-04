import { Routes } from '@angular/router';

// Import feature components
import { HomeComponent } from './features/home/home.component';
import { IncomeComponent } from './features/income/income.component';
import { ExpenseComponent } from './features/expense/expense.component';
import { BudgetComponent } from './features/budget/budget.component';
import { AnalyticsComponent } from './features/analytics/analytics.component';
import { CsvComponent } from './features/csv/csv.component';

export const routes: Routes = [
  {
    path: '',
    component: HomeComponent,
    title: 'ExpenseMate - Dashboard'
  },
  {
    path: 'income',
    component: IncomeComponent,
    title: 'ExpenseMate - Income Management'
  },
  {
    path: 'expenses',
    component: ExpenseComponent,
    title: 'ExpenseMate - Expense Tracker'
  },
  {
    path: 'budgets',
    component: BudgetComponent,
    title: 'ExpenseMate - Budget & Alerts'
  },
  {
    path: 'analytics',
    component: AnalyticsComponent,
    title: 'ExpenseMate - Analytics & Visualization'
  },
  {
    path: 'csv',
    component: CsvComponent,
    title: 'ExpenseMate - Import / Export CSV'
  },
  {
    path: '**',
    redirectTo: ''
  }
];
