using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseMate.Database;
using Microsoft.EntityFrameworkCore;
using ExpenseMate.Domain;

public class ExpenseMateDbContext:DbContext
{
    public ExpenseMateDbContext(DbContextOptions<ExpenseMateDbContext> options)
        : base(options)
    {
        
    }

    public DbSet<Income> Incomes { get; set; } = null!;
    public DbSet<Expense> Expenses { get; set; } = null!;
    public DbSet<Budget> Budgets { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Ensures EF Core maps to exact SQL table names
        modelBuilder.Entity<Income>().ToTable("Income");
        modelBuilder.Entity<Expense>().ToTable("Expense");
        modelBuilder.Entity<Budget>().ToTable("Budget");
    }
}
