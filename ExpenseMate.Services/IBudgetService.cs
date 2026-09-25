namespace ExpenseMate.Services;
using ExpenseMate.Domain;

public interface IBudgetService
{
    Task<Budget?> GetBudgetByMonthAsync(string month);
    Task SetBudgetAsync(Budget budget);
    Task UpdateBudgetAsync(Budget budget);
    Task DeleteBudgetAsync(int id);
}