import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="home-container">
      <div class="welcome-header">
        <h1>Welcome to ExpenseMate</h1>
        <p>Your personal financial control center.</p>
      </div>

      <div class="cards-grid">
        <!-- 1. Income Management -->
        <div class="card">
          <div class="card-header">
            <span class="card-icon">💵</span>
            <h3>Income Management</h3>
          </div>
          <p class="card-description">
            Track earnings, view sources, and search individual records by ID.
          </p>
          <a routerLink="/income" class="card-btn income-btn">
            Go to Income &rarr;
          </a>
        </div>

        <!-- 2. Expense Tracker -->
        <div class="card">
          <div class="card-header">
            <span class="card-icon">💳</span>
            <h3>Expense Tracker</h3>
          </div>
          <p class="card-description">
            Monitor day-to-day spending and organize expenses by category.
          </p>
          <a routerLink="/expenses" class="card-btn expense-btn">
            Go to Expenses &rarr;
          </a>
        </div>

        <!-- 3. Budget & Alerts -->
        <div class="card">
          <div class="card-header">
            <span class="card-icon">🎯</span>
            <h3>Budget & Alerts</h3>
          </div>
          <p class="card-description">
            Set monthly spending limits and check real-time threshold alerts.
          </p>
          <a routerLink="/budgets" class="card-btn budget-btn">
            Go to Budgets &rarr;
          </a>
        </div>

        <!-- 4. Analytics & Visualization -->
        <div class="card">
          <div class="card-header">
            <span class="card-icon">📊</span>
            <h3>Analytics & Visualization</h3>
          </div>
          <p class="card-description">
            View income vs. expense visual charts and category percentage breakdowns[cite: 10].
          </p>
          <a routerLink="/analytics" class="card-btn analytics-btn">
            Go to Analytics &rarr;
          </a>
        </div>

        <!-- 5. CSV Data Management -->
        <div class="card">
          <div class="card-header">
            <span class="card-icon">📁</span>
            <h3>CSV Data Management</h3>
          </div>
          <p class="card-description">
            Export transaction records to CSV or bulk-import transactions into the database[cite: 10].
          </p>
          <a routerLink="/csv" class="card-btn csv-btn">
            Import / Export CSV &rarr;
          </a>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .home-container {
      padding: 32px 24px;
      color: #f3f4f6;
      max-width: 1400px;
      margin: 0 auto;
    }

    .welcome-header h1 {
      font-size: 2rem;
      font-weight: 700;
      margin: 0 0 8px 0;
      color: #ffffff;
    }

    .welcome-header p {
      color: #9ca3af;
      margin: 0 0 32px 0;
      font-size: 1rem;
    }

    .cards-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(320px, 1fr));
      gap: 24px;
    }

    .card {
      background-color: #111827;
      border: 1px solid #1f2937;
      border-radius: 12px;
      padding: 24px;
      display: flex;
      flex-direction: column;
      justify-content: space-between;
      transition: transform 0.2s ease, border-color 0.2s ease;
    }

    .card:hover {
      border-color: #374151;
      transform: translateY(-2px);
    }

    .card-header {
      display: flex;
      align-items: center;
      gap: 12px;
      margin-bottom: 12px;
    }

    .card-icon {
      font-size: 1.25rem;
    }

    .card-header h3 {
      font-size: 1.15rem;
      font-weight: 600;
      margin: 0;
      color: #f9fafb;
    }

    .card-description {
      color: #9ca3af;
      font-size: 0.9rem;
      line-height: 1.5;
      margin: 0 0 24px 0;
      flex-grow: 1;
    }

    .card-btn {
      display: flex;
      align-items: center;
      justify-content: center;
      padding: 10px 16px;
      border-radius: 8px;
      font-weight: 600;
      font-size: 0.9rem;
      text-decoration: none;
      transition: opacity 0.2s ease;
      cursor: pointer;
    }

    .card-btn:hover {
      opacity: 0.9;
    }

    /* Button Variations */
    .income-btn {
      background-color: #10b981;
      color: #042f2e;
    }

    .expense-btn {
      background-color: #38bdf8;
      color: #082f49;
    }

    .budget-btn {
      background-color: #1f2937;
      color: #e5e7eb;
      border: 1px solid #374151;
    }

    .analytics-btn {
      background-color: #a855f7;
      color: #3b0764;
    }

    .csv-btn {
      background-color: #06b6d4;
      color: #164e63;
    }
  `]
})
export class HomeComponent {}
