namespace ExpenseMate.Domain;

public interface IBudgetRepo{
    Task <Budget?> GetByMonth(string month);
    Task<Budget> AddOrUpdateBudgetAsync(Budget budget);
    Task DeleteBudgetAsync(int id);
}