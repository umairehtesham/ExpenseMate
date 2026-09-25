namespace ExpenseMate.Domain;

public class Budget{
    public int BudgetId {get;set;}
    public float BudgetAmount {get;set;}
    public float AlertPercent {get;set;}
    public string BudgetMonth {get;set;} = string.Empty;
}