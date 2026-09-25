namespace ExpenseMate.Services;
using ExpenseMate.Domain;

public interface IExpenseService
{
    Task<IEnumerable<Expense>> GetAllExpensesAsync();
    Task<Expense?> GetExpenseByIdAsync(int id);
    Task AddExpenseAsync(Expense expense);
    Task UpdateExpenseAsync(Expense expense);
    Task DeleteExpenseAsync(int id);
    Task<double> GetTotalSpentByMonthAsync(string month);
}
