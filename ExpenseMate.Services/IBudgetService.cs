namespace ExpenseMate.Services;
using ExpenseMate.Domain;

public interface IBudgetService
{
    Task<Budget?> GetBudgetByMonthAsync(string month);
     Task<Budget> SaveBudgetAsync(Budget budget);
    
}