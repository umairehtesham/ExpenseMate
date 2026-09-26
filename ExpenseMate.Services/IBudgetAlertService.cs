namespace ExpenseMate.Services;
using ExpenseMate.Domain;
public interface IBudgetAlertService
{
 Task<BudgetAlertDto> CheckBudgetAlertAsync(string month);
}