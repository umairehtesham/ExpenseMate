namespace ExpenseMate.Services;
using ExpenseMate.Domain;

public class ExpenseService : IExpenseService
{
    private readonly IExpenseRepo _expenseRepo;

    public ExpenseService(IExpenseRepo er)
    {
        _expenseRepo = er;        
    }

    public async Task<IEnumerable<Expense>> GetAllExpensesAsync()
    {
        return await _expenseRepo.GetAllAsync();
    }

    public async Task<Expense?> GetExpenseByIdAsync(int id)
    {
        if(id <= 0)
            throw new ArgumentException("Expense Id is not valid");
        return await _expenseRepo.GetByIdAsync(id);
    }

    public async Task AddExpenseAsync(Expense expense)
    {
        if (expense.ExpenseAmount <= 0)
            throw new ArgumentException("Expense Amount is not invalid");

        await _expenseRepo.AddAsync(expense);
    }

    public async Task UpdateExpenseAsync(Expense expense)
    {
        if (expense.ExpenseId <= 0)
            throw new ArgumentException("Expense Id is not valid");

        if (expense.ExpenseAmount <= 0 )
            throw new ArgumentException("Expense Amount is not invalid");
        
        await _expenseRepo.UpdateAsync(expense);
    }
    public async Task DeleteExpenseAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("Expense Id is not valid");

        await _expenseRepo.DeleteAsync(id);
    }

    public async Task<double> GetTotalSpentByMonthAsync(string month)
    {
        if (string.IsNullOrWhiteSpace(month))
            throw new ArgumentException("Month not entered");
        
        return await _expenseRepo.GetTotalSpentByMonthAsync(month);
    }
}