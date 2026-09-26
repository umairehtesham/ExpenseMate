export interface BudgetAlertResponse {
  month: string;
  budgetLimit: number;
  totalSpent: number;
  isAlertTriggered: boolean;
  spentPercentage: number;
  message: string;
}