namespace ExpenseMate.Database;

using ExpenseMate.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;

public class ExpenseRepo : IExpenseRepo
{
    private readonly ExpenseMateDbContext _context;

    public ExpenseRepo(ExpenseMateDbContext context)
    {
        _context=context;
    }

    public async Task<IEnumerable<Expense>> GetAllAsync()
    {
        return await _context.Expenses.AsNoTracking().ToListAsync();
    }

    public async Task<Expense?> GetByIdAsync(int id)
    {
        return await _context.Expenses.FindAsync(id);
    }

    public async Task AddAsync(Expense Expense)
    {
        await _context.Expenses.AddAsync(Expense);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Expense Expense)
    {
        _context.Expenses.Update(Expense); 
        await _context.SaveChangesAsync();
    } 

    public async Task DeleteAsync(int id)
    {
        var Expense=await _context.Expenses.FindAsync(id);
        if(Expense is not null)
        {
            _context.Expenses.Remove(Expense);
            await _context.SaveChangesAsync();
        }
    }

   public async Task<double> GetTotalSpentByMonthAsync(string month)
{
    if (string.IsNullOrWhiteSpace(month)) return 0;

    // Fetch expenses into memory
    var expenses = await _context.Expenses.ToListAsync();

    // Change 'Date' to match your Expense.cs property (e.g., e.Date, e.DateTime, or e.CreatedAt)
    double totalSpent = expenses
        .Where(e => e.CreatedAt.ToString("MMMM").Equals(month.Trim(), StringComparison.OrdinalIgnoreCase) ||
                    e.CreatedAt.ToString("MMM").Equals(month.Trim(), StringComparison.OrdinalIgnoreCase))
        .Sum(e => (double)Math.Abs(e.ExpenseAmount));

    return totalSpent;
}
}
    