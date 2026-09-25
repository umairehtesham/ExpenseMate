namespace ExpenseMate.Domain;

public interface IExpenseRepo{
    Task<IEnumerable<Expense>> GetAllAsync();
    Task<Expense?> GetByIdAsync(int id);
    Task AddAsync(Expense expense);
    Task UpdateAsync(Expense expense);
    Task DeleteAsync(int id);
    Task<double> GetTotalSpentByMonthAsync(string month);
}