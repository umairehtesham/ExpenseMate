namespace ExpenseMate.Services;
using ExpenseMate.Domain;

public class BudgetService : IBudgetService
{
    private readonly IBudgetRepo _budgetRepo;

    public BudgetService(IBudgetRepo budgetRepo)
    {
        _budgetRepo=budgetRepo;
    }

    public async Task<Budget?> GetBudgetByMonthAsync(string month)
    {
        if (string.IsNullOrWhiteSpace(month))
        {
            throw new ArgumentException("Month not entered");
        }

        return await _budgetRepo.GetByMonth(month);
    }

    public async Task SetBudgetAsync(Budget budget)
    {
        if (budget.BudgetAmount <= 0)
        {
            throw new ArgumentException("Budget amount cannot be negative or zero");
        }
        if (budget.AlertPercent <= 0)
        {
            throw new ArgumentException("Budget Alert Percent cannot be negative or zero");   
        }
        if (string.IsNullOrWhiteSpace(budget.BudgetMonth))
        {
             throw new ArgumentException("Budget month cannot be  null");   
        }
        await _budgetRepo.AddBudgetAsync(budget);
    }

    public async Task UpdateBudgetAsync(Budget budget)
    {
        if (budget.BudgetId <= 0)
        {
            throw new ArgumentException("Budget Id not valid");
        }
        if (budget.BudgetAmount <= 0)
        {
            throw new ArgumentException("Budget amount cannot be negative or zero");
        }
        if (budget.AlertPercent <= 0)
        {
            throw new ArgumentException("Budget Alert Percent cannot be negative or zero");   
        }
        if (string.IsNullOrWhiteSpace(budget.BudgetMonth))
        {
             throw new ArgumentException("Budget month cannot be  null");   
        }
        await _budgetRepo.UpdateBudgetAsync(budget);
    }
    public async Task DeleteBudgetAsync(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException("Budget Id not valid");
        }
        await _budgetRepo.DeleteBudgetAsync(id);
    }

}