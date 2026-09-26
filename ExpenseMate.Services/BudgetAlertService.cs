using ExpenseMate.Domain;
using System;
using System.Threading.Tasks;

namespace ExpenseMate.Domain
{
    public class BudgetAlertDto
    {
        public bool IsAlertTriggered { get; set; }
        public decimal TotalSpent { get; set; }
        public decimal BudgetLimit { get; set; }
        public decimal SpentPercentage { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}

namespace ExpenseMate.Services
{
    public class BudgetAlertService : IBudgetAlertService
    {
        private readonly IBudgetRepo _budgetRepo;
        private readonly IExpenseRepo _expenseRepo;

        public BudgetAlertService(IBudgetRepo budgetRepo, IExpenseRepo expenseRepo)
        {
            _budgetRepo = budgetRepo;
            _expenseRepo = expenseRepo;
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

                // 1. Convert to decimal and apply Math.Abs to handle negative values (-90.00 -> 90.00)
                decimal rawSpent = Convert.ToDecimal(await _expenseRepo.GetTotalSpentByMonthAsync(month));
                decimal totalSpent = Math.Abs(rawSpent);

                // 2. Perform exact decimal calculations
                decimal spentPercentage = (totalSpent / (decimal)budget.BudgetAmount) * 100m;

                // 3. Cast float to decimal to allow comparison
                bool isAlertTriggered = spentPercentage >= (decimal)budget.AlertPercent;

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
}