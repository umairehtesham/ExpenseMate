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

    
    public async Task AddBudgetAsync(Budget Budget)
    {
        await _context.Budgets.AddAsync(Budget);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateBudgetAsync(Budget Budget)
    {
        _context.Budgets.Update(Budget); 
        await _context.SaveChangesAsync();
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
    