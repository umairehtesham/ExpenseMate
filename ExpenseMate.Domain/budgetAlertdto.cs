namespace ExpenseMate.Domain;

public class budgetAlertdto
{
    public bool IsAlertTriggered { get; set; }
    public decimal TotalSpent { get; set; }
    public decimal BudgetAmount { get; set; }
    public decimal SpentPercentage { get; set; }
    public decimal AlertPercent { get; set; }
    public string Message { get; set; } = string.Empty;
}