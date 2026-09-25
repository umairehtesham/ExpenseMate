namespace ExpenseMate.Domain;


public class Expense{
    public int ExpenseId {get; set;}
    public float ExpenseAmount{get; set;}
    public string ExpenseCategory{get;set;} = string.Empty;
    public DateTime CreatedAt {get;set;}=DateTime.UtcNow;
}