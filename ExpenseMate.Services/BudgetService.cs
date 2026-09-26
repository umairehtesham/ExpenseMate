using ExpenseMate.Domain;
using System;
using System.Threading.Tasks;

namespace ExpenseMate.Services
{
    public class BudgetService : IBudgetService
    {
        private readonly IBudgetRepo _budgetRepo;

        public BudgetService(IBudgetRepo budgetRepo)
        {
            _budgetRepo = budgetRepo;
        }

        public async Task<Budget?> GetBudgetByMonthAsync(string month)
        {
            if (string.IsNullOrWhiteSpace(month))
            {
                throw new ArgumentException("Month not entered");
            }

            return await _budgetRepo.GetByMonth(month);
        }

        public async Task<Budget> SaveBudgetAsync(Budget budget)
        {
            if (budget == null || string.IsNullOrWhiteSpace(budget.BudgetMonth))
            {
                throw new ArgumentException("Budget month is required.");
            }

            // Calls the repo where _context actually lives
            return await _budgetRepo.AddOrUpdateBudgetAsync(budget);
        }
    }
}