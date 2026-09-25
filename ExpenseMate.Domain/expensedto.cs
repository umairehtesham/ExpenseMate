namespace ExpenseMate.Domain;

//read
public record expenseRecord(int ExpenseId,float ExpenseAmount,string ExpenseCategory,DateTime CreatedAt);

//write
public record CreateExpenseRequest(float ExpenseAmount,string ExpenseCategory);