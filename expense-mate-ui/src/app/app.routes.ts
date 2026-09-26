import { Routes } from '@angular/router';
import { HomeComponent } from './features/home/home.component';
import { IncomeComponent } from './features/income/income.component';
import { ExpenseComponent } from './features/expense/expense.component';
import { BudgetComponent } from './features/budget/budget.component';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'income', component: IncomeComponent },
  { path: 'expenses', component: ExpenseComponent },
  { path: 'budgets', component: BudgetComponent },
  { path: '**', redirectTo: '' }
];