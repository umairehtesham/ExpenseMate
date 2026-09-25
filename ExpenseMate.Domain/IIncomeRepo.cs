namespace ExpenseMate.Domain;

public interface IIncomeRepo{
    Task<IEnumerable<Income>> GetAllAsync();
    Task<Income?> GetByIdAsync(int id);
    Task AddAsync(Income income);
    Task UpdateAsync(Income income);
    Task DeleteAsync(int id);
}