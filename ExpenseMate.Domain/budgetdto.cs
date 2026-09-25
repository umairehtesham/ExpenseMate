namespace ExpenseMate.Domain;


public record budgetRecord(int id,float BudgetAmount,float AlertPercent,string BudgetMonth);


public record CreateBudgetRequest(float BudgetAmount,float AlertPercent,string BudgetMonth);