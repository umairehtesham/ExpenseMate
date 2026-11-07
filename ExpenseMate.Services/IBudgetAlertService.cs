namespace ExpenseMate.Services;
using ExpenseMate.Domain;
public interface IBudgetAlertService
{
 Task<budgetAlertdto> CheckBudgetAlertAsync(string month);
}