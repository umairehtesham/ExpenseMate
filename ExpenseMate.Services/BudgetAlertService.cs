using System;
using System.Threading.Tasks;
using ExpenseMate.Domain;


namespace ExpenseMate.Services;

public class BudgetAlertService : IBudgetAlertService
{
    private readonly IBudgetRepo _budgetRepo;
    private readonly IExpenseRepo _expenseRepo;

    public BudgetAlertService(IBudgetRepo budgetRepo, IExpenseRepo expenseRepo)
    {
        _budgetRepo = budgetRepo;
        _expenseRepo = expenseRepo;
    }

    public async Task<budgetAlertdto> CheckBudgetAlertAsync(string month)
    {
        // 1. Fetch budget record
        var budget = await _budgetRepo.GetByMonth(month);

        // 2. Simple guard for missing budget
        if (budget == null)
        {
            return new budgetAlertdto
            {
                IsAlertTriggered = false,
                Message = $"No budget record configured for {month}."
            };
        }

        // 3. Simple guard for non-positive budget limit
        if (budget.BudgetAmount <= 0)
        {
            return new budgetAlertdto
            {
                IsAlertTriggered = false,
                Message = "Budget amount must be greater than zero."
            };
        }

        // 4. Get spent amount and convert negative inputs to positive
        decimal rawSpent = Convert.ToDecimal(await _expenseRepo.GetTotalSpentByMonthAsync(month));
        decimal totalSpent = Math.Abs(rawSpent);

        // 5. Compute percentage and threshold
        decimal spentPercentage = Math.Round((totalSpent / (decimal)budget.BudgetAmount) * 100m, 2);
        decimal alertThreshold = budget.AlertPercent > 0 ? (decimal)budget.AlertPercent : 80m;
        bool isTriggered = spentPercentage >= alertThreshold;

        // 6. Return standard result DTO
        return new budgetAlertdto
        {
            IsAlertTriggered = isTriggered,
            TotalSpent = totalSpent,
            BudgetAmount = (decimal)budget.BudgetAmount,
            SpentPercentage = spentPercentage,
            AlertPercent = alertThreshold,
            Message = isTriggered
                ? $"Alert! You used {spentPercentage}% of your ${budget.BudgetAmount:F2} budget."
                : $"Within budget. You used {spentPercentage}% of your ${budget.BudgetAmount:F2} budget."
        };
    }
}