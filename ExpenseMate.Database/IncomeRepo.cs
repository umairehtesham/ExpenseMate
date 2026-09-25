namespace ExpenseMate.Database;
using ExpenseMate.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public class IncomeRepo : IIncomeRepo
{
    private readonly ExpenseMateDbContext _context;

    public IncomeRepo(ExpenseMateDbContext context)
    {
        _context=context;
    }

    public async Task<IEnumerable<Income>> GetAllAsync()
    {
        return await _context.Incomes.AsNoTracking().ToListAsync();
    }

    public async Task<Income?> GetByIdAsync(int id)
    {
        return await _context.Incomes.FindAsync(id);
    }

    public async Task AddAsync(Income income)
    {
        await _context.Incomes.AddAsync(income);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Income income)
    {
        _context.Incomes.Update(income); 
        await _context.SaveChangesAsync();
    } 

    public async Task DeleteAsync(int id)
    {
        var income=await _context.Incomes.FindAsync(id);
        if(income is not null)
        {
            _context.Incomes.Remove(income);
            await _context.SaveChangesAsync();
        }
    }
}
    
