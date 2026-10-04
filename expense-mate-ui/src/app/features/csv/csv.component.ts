import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CsvService } from '../../core/services/csv.service';
import { CsvImportResponse } from '../../core/models/csv.model';

@Component({
  selector: 'app-csv',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="csv-container">
      <h2>CSV Data Management</h2>

      <!-- Card 1: Export CSV -->
      <div class="card">
        <h3>Export Transactions to CSV</h3>
        <p class="description">Download your database transactions into standard CSV files[cite: 10].</p>
        <div class="button-group">
          <button (click)="exportData('all')" class="btn primary">Export All Data</button>
          <button (click)="exportData('incomes')" class="btn secondary">Export Incomes Only</button>
          <button (click)="exportData('expenses')" class="btn secondary">Export Expenses Only</button>
        </div>
      </div>

      <!-- Card 2: Import CSV -->
      <div class="card">
        <h3>Import CSV File into Database</h3>
        <p class="description">Upload a <code>.csv</code> file to bulk-insert transactions[cite: 10].</p>
        <span class="format-note">Expected Format: <code>Type, Amount, Category, Date</code></span>

        <div class="upload-section">
          <input type="file" (change)="onFileSelected($event)" accept=".csv" #fileInput />
          <button (click)="uploadCsv()" [disabled]="!selectedFile || isLoading" class="btn primary">
            {{ isLoading ? 'Uploading...' : 'Upload & Import' }}
          </button>
        </div>

        <div *ngIf="uploadResult" class="result-message" [ngClass]="{ 'success': uploadResult.isSuccess, 'error': !uploadResult.isSuccess }">
          <p>{{ uploadResult.message }}</p>
          <ul *ngIf="uploadResult.isSuccess">
            <li>Total Inserted: {{ uploadResult.insertedCount ?? 0 }}</li>
            <li>Incomes: {{ uploadResult.incomesInserted ?? 0 }}</li>
            <li>Expenses: {{ uploadResult.expensesInserted ?? 0 }}</li>
            <li *ngIf="(uploadResult?.skippedRows ?? 0) > 0">Skipped Invalid Rows: {{ uploadResult?.skippedRows }}</li>
          </ul>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .csv-container { padding: 24px; max-width: 800px; margin: 0 auto; color: #fff; }
    .card { background: #1e1e1e; padding: 24px; border-radius: 8px; margin-bottom: 24px; }
    .description { color: #aaa; font-size: 0.9rem; margin-bottom: 12px; }
    .format-note { display: block; font-size: 0.8rem; color: #00bcd4; margin-bottom: 16px; }
    .button-group { display: flex; gap: 12px; flex-wrap: wrap; }
    .upload-section { display: flex; gap: 12px; align-items: center; }
    .btn { padding: 8px 16px; border: none; border-radius: 4px; cursor: pointer; font-weight: bold; }
    .btn.primary { background: #00bcd4; color: #000; }
    .btn.secondary { background: #333; color: #fff; }
    .btn:disabled { opacity: 0.5; cursor: not-allowed; }
    .result-message { margin-top: 16px; padding: 12px; border-radius: 4px; }
    .success { background-color: #1b3a2b; color: #4caf50; }
    .error { background-color: #3a1b1b; color: #f44336; }
  `]
})
export class CsvComponent {
  private csvService = inject(CsvService);

  selectedFile: File | null = null;
  isLoading = false;
  uploadResult: CsvImportResponse | null = null;

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.selectedFile = input.files[0];
    }
  }

  uploadCsv(): void {
    if (!this.selectedFile) return;

    this.isLoading = true;
    this.csvService.importCsv(this.selectedFile).subscribe({
      next: (res) => {
        this.uploadResult = res;
        this.isLoading = false;
      },
      error: (err) => {
        this.uploadResult = {
          isSuccess: false,
          message: err.error?.message || 'Failed to upload and parse CSV file.'
        };
        this.isLoading = false;
      }
    });
  }

  exportData(type: string): void {
    this.csvService.exportCsv(type).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `ExpenseMate_${type}_${new Date().toISOString().split('T')[0]}.csv`;
        a.click();
        window.URL.revokeObjectURL(url);
      }
    });
  }
}
