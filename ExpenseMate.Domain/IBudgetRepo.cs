namespace ExpenseMate.Domain;

public interface IBudgetRepo{
    Task <Budget?> GetByMonth(string month);
    Task AddBudgetAsync(Budget budget);
    Task UpdateBudgetAsync(Budget budget);
    Task DeleteBudgetAsync(int id);
}