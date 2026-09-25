namespace ExpenseMate.Services;
using ExpenseMate.Domain;

public interface IIncomeService
{
    Task<IEnumerable<Income>> GetAllIncomeAsync();
    Task<Income?> GetIncomeByIdAsync(int id);
    Task AddIncomeAsync(Income income);
    Task UpdateIncomeAsync(Income income);
    Task DeleteIncomeAsync(int id);
}