export interface CategoryTotalDto {
  category: string;
  totalAmount: number;
  percentage: number;
}


export interface MonthlyOverviewDto {
  month: number;
  monthName: string;
  year: number;
  totalIncome: number;
  totalExpense: number;
}


export interface AnalyticsSummaryDto {
  totalIncome: number;
  totalExpense: number;
  netSavings: number;
  categoryBreakdown: CategoryTotalDto[];
  monthlyOverview: MonthlyOverviewDto[];
}
