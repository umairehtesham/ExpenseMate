namespace ExpenseMate.Services;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExpenseMate.Domain;

public class IncomeService : IIncomeService
{
    private readonly IIncomeRepo _incomeRepo;

    public IncomeService(IIncomeRepo incomeRepo)
    {
        _incomeRepo = incomeRepo ?? throw new ArgumentNullException(nameof(incomeRepo));
    }

    public async Task<IEnumerable<Income>> GetAllIncomeAsync()
    {
        return await _incomeRepo.GetAllAsync();
    }

    public async Task<Income?> GetIncomeByIdAsync(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException("Income ID must be greater than zero.", nameof(id));
        }

        return await _incomeRepo.GetByIdAsync(id);
    }

    public async Task AddIncomeAsync(Income income)
    {
        if (income == null)
        {
            throw new ArgumentNullException(nameof(income), "Income object cannot be null.");
        }

        if (income.IncomeAmount <= 0)
        {
            throw new ArgumentException("Income amount must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(income.Category))
        {
            throw new ArgumentException("Category is required.");
        }

        await _incomeRepo.AddAsync(income);
    }

    public async Task UpdateIncomeAsync(Income income)
    {
        if (income == null)
        {
            throw new ArgumentNullException(nameof(income), "Income object cannot be null.");
        }

        if (income.IncomeId <= 0)
        {
            throw new ArgumentException("Income ID must be greater than zero.", nameof(income.IncomeId));
        }

        if (income.IncomeAmount <= 0)
        {
            throw new ArgumentException("Income amount must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(income.Category))
        {
            throw new ArgumentException("Category is required.");
        }

        await _incomeRepo.UpdateAsync(income);
    }

    public async Task DeleteIncomeAsync(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException("Income ID must be greater than zero.", nameof(id));
        }

        await _incomeRepo.DeleteAsync(id);
    }
}