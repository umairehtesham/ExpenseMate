namespace BudgetMate.Database;

using ExpenseMate.Database;
using ExpenseMate.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;

public class BudgetRepo : IBudgetRepo
{
    private readonly ExpenseMateDbContext _context;

    public BudgetRepo(ExpenseMateDbContext context)
    {
        _context=context;
    }

    public async Task<Budget?> GetByMonth(string month)
    {
        return await _context.Budgets.FirstOrDefaultAsync(b=>b.BudgetMonth==month);
    }

    
    public async Task<Budget> AddOrUpdateBudgetAsync(Budget budget)
    {
        // 1. Check if a budget record already exists for this month
        var existingBudget = await _context.Budgets
            .FirstOrDefaultAsync(b => b.BudgetMonth.Trim().ToLower() == budget.BudgetMonth.Trim().ToLower());

        if (existingBudget != null)
        {
            // 2. UPDATE existing row
            existingBudget.BudgetAmount = budget.BudgetAmount;
            existingBudget.AlertPercent = budget.AlertPercent;

            _context.Budgets.Update(existingBudget);
            await _context.SaveChangesAsync();
            return existingBudget;
        }
        else
        {
            // 3. INSERT new row
            await _context.Budgets.AddAsync(budget);
            await _context.SaveChangesAsync();
            return budget;
        }
    }

    public async Task DeleteBudgetAsync(int id)
    {
        var Budget=await _context.Budgets.FindAsync(id);
        if(Budget is not null)
        {
            _context.Budgets.Remove(Budget);
            await _context.SaveChangesAsync();
        }
    }

    
}
    