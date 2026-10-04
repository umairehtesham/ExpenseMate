export interface CsvImportResponse {
  message?: string;
  insertedCount?: number;
  incomesInserted?: number;
  expensesInserted?: number;
  skippedRows?: number;
  isSuccess?: boolean;
}
