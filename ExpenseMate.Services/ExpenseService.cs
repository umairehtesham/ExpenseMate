namespace ExpenseMate.Services;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExpenseMate.Domain;

public class ExpenseService : IExpenseService
{
    private readonly IExpenseRepo _expenseRepo;

    public ExpenseService(IExpenseRepo expenseRepo)
    {
        _expenseRepo = expenseRepo ?? throw new ArgumentNullException(nameof(expenseRepo));
    }

    public async Task<IEnumerable<Expense>> GetAllExpensesAsync()
    {
        return await _expenseRepo.GetAllAsync();
    }

    public async Task<Expense?> GetExpenseByIdAsync(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException("Expense ID must be greater than zero.", nameof(id));
        }

        return await _expenseRepo.GetByIdAsync(id);
    }

    public async Task AddExpenseAsync(Expense expense)
    {
        if (expense == null)
        {
            throw new ArgumentNullException(nameof(expense), "Expense object cannot be null.");
        }

        if (expense.ExpenseAmount <= 0)
        {
            throw new ArgumentException("Expense amount must be greater than zero.");
        }

        await _expenseRepo.AddAsync(expense);
    }

    public async Task UpdateExpenseAsync(Expense expense)
    {
        if (expense == null)
        {
            throw new ArgumentNullException(nameof(expense), "Expense object cannot be null.");
        }

        if (expense.ExpenseId <= 0)
        {
            throw new ArgumentException("Expense ID must be greater than zero.", nameof(expense.ExpenseId));
        }

        if (expense.ExpenseAmount <= 0)
        {
            throw new ArgumentException("Expense amount must be greater than zero.");
        }

        await _expenseRepo.UpdateAsync(expense);
    }

    public async Task DeleteExpenseAsync(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException("Expense ID must be greater than zero.", nameof(id));
        }

        await _expenseRepo.DeleteAsync(id);
    }

    public async Task<double> GetTotalSpentByMonthAsync(string month)
    {
        if (string.IsNullOrWhiteSpace(month))
        {
            throw new ArgumentException("A valid month must be provided.", nameof(month));
        }

        return await _expenseRepo.GetTotalSpentByMonthAsync(month);
    }
}