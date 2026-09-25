namespace ExpenseMate.Domain;


public record IncomeRecord(int IncomeId,float IncomeAmount,string Category,DateTime CreatedAt);

public record CreateIncomeRequest(float IncomeAmount,string Category);