namespace ExpenseMate.Domain;

public class Income{
   public int IncomeId {get; set;}
    public float IncomeAmount{get; set;}
    public string Category{get;set;} = string.Empty;
    public DateTime CreatedAt {get;set;}=DateTime.UtcNow;
}