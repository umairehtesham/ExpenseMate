namespace ExpenseMate.Services;
using ExpenseMate.Domain;

public class BudgetAlertDto
{
    public bool  IsAlertTriggered{get;set;}
    public double TotalSpent{get;set;}
    public decimal BudgetLimit{get;set;}
    public double SpentPercentage{get;set;}
    public string Message{get;set;}=String.Empty;
}

public class BudgetAlertService : IBudgetAlertService
{
    private readonly IExpenseRepo _expenseRepo;
    private readonly IBudgetRepo _budgetRepo;
    public BudgetAlertService(IExpenseRepo expenseRepository, IBudgetRepo budgetRepository)
    {
        _expenseRepo = expenseRepository;
        _budgetRepo = budgetRepository;
    }

    public async Task<BudgetAlertDto> CheckBudgetAlertAsync(string month)
    {
        var budget = await _budgetRepo.GetByMonth(month);
        
        if (budget is null || budget.BudgetAmount <= 0)
        {
            return new BudgetAlertDto
            {
                IsAlertTriggered = false,
                Message = $"No budget record configured for {month}."
            };
        }

        double totalSpent = await _expenseRepo.GetTotalSpentByMonthAsync(month);
        double spentPercentage = (totalSpent / (double)budget.BudgetAmount) * 100;
        bool isAlertTriggered = spentPercentage >= budget.AlertPercent;

        return new BudgetAlertDto
        {
            IsAlertTriggered = isAlertTriggered,
            TotalSpent = totalSpent,
            BudgetLimit = (decimal)budget.BudgetAmount,
            SpentPercentage = Math.Round(spentPercentage, 2),
            Message = isAlertTriggered
                ? $"Alert triggered! You have spent {spentPercentage:F1}% of your limit (Threshold: {budget.AlertPercent}%)."
                : $"Spending is within safe limits ({spentPercentage:F1}% spent)."
        };
    }
}