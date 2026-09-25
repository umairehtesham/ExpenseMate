namespace ExpenseMate.Services;

using System.Security;
using ExpenseMate.Domain;

public class IncomeService : IIncomeService
{
    private readonly IIncomeRepo _incomeRepo;

    public IncomeService(IIncomeRepo incomeRepo)
    {
        _incomeRepo=incomeRepo;
    }

    public async Task<IEnumerable<Income>> GetAllIncomeAsync()
    {
        return await _incomeRepo.GetAllAsync();
    }

    public async Task<Income?> GetIncomeByIdAsync(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException("Income ID not valid");
        }
        return await _incomeRepo.GetByIdAsync(id);
    }

    public async Task AddIncomeAsync(Income income)
    {
        if (income.IncomeAmount < 0)
        {
            throw new ArgumentException("Income amount not valid");
        }
        if (string.IsNullOrWhiteSpace(income.Category))
        {
            throw new ArgumentException("Category not entered");
        }

        await _incomeRepo.AddAsync(income);
    }

    public async Task UpdateIncomeAsync(Income income)
    {
        if (income.IncomeId <= 0)
        {
            throw new ArgumentException("Income Id not valid");
        }
        if (income.IncomeAmount<0)
        {
            throw new ArgumentException("Income amount not valid");
        }

        await _incomeRepo.UpdateAsync(income);
    }

    public async Task DeleteIncomeAsync(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException("Income Id not valid");
        }
        await _incomeRepo.DeleteAsync(id);
    }
}