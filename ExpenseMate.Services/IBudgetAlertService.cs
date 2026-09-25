namespace ExpenseMate.Services;

public interface IBudgetAlertService
{
    Task<BudgetAlertDto> CheckBudgetAlertAsync(string month);
}