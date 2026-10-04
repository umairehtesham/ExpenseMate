using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpenseMate.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ExpenseMate.Api.Controllers;

public class AnalyticsSummaryDto
{
    [JsonPropertyName("totalIncome")]
    public decimal TotalIncome { get; set; }

    [JsonPropertyName("totalExpense")]
    public decimal TotalExpense { get; set; }

    [JsonPropertyName("netSavings")]
    public decimal NetSavings { get; set; }

    [JsonPropertyName("categoryBreakdown")]
    public List<CategoryTotalDto> CategoryBreakdown { get; set; } = new();

    [JsonPropertyName("monthlyOverview")]
    public List<MonthlyOverviewDto> MonthlyOverview { get; set; } = new();
}

public class CategoryTotalDto
{
    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;

    [JsonPropertyName("totalAmount")]
    public decimal TotalAmount { get; set; }

    [JsonPropertyName("percentage")]
    public double Percentage { get; set; }
}

public class MonthlyOverviewDto
{
    [JsonPropertyName("month")]
    public int Month { get; set; }

    [JsonPropertyName("monthName")]
    public string MonthName { get; set; } = string.Empty;

    [JsonPropertyName("year")]
    public int Year { get; set; }

    [JsonPropertyName("totalIncome")]
    public decimal TotalIncome { get; set; }

    [JsonPropertyName("totalExpense")]
    public decimal TotalExpense { get; set; }
}

[ApiController]
[Route("api/[controller]")]
public class AnalyticsController : ControllerBase
{
    private readonly ExpenseMateDbContext _context;

    public AnalyticsController(ExpenseMateDbContext context)
    {
        _context = context;
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetAnalyticsSummary([FromQuery] int? year)
    {
        try
        {
            int targetYear = year ?? DateTime.UtcNow.Year;

            var allIncomes = await _context.Incomes.AsNoTracking().ToListAsync();
            var allExpenses = await _context.Expenses.AsNoTracking().ToListAsync();

            var incomes = allIncomes.Where(i => i.CreatedAt != default && i.CreatedAt.Year == targetYear).ToList();
            var expenses = allExpenses.Where(e => e.CreatedAt != default && e.CreatedAt.Year == targetYear).ToList();

            decimal totalIncome = incomes.Sum(i => (decimal)i.IncomeAmount);
            decimal totalExpense = expenses.Sum(e => (decimal)e.ExpenseAmount);

            var categoryBreakdown = expenses
                .GroupBy(e => string.IsNullOrWhiteSpace(e.ExpenseCategory) ? "Uncategorized" : e.ExpenseCategory.Trim())
                .Select(g =>
                {
                    decimal categoryTotal = g.Sum(e => (decimal)e.ExpenseAmount);
                    return new CategoryTotalDto
                    {
                        Category = g.Key,
                        TotalAmount = categoryTotal,
                        Percentage = totalExpense > 0 ? (double)Math.Round((categoryTotal / totalExpense) * 100, 2) : 0
                    };
                })
                .OrderByDescending(c => c.TotalAmount)
                .ToList();

            string[] monthNames = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

            var monthlyOverview = Enumerable.Range(1, 12)
                .Select(m => new MonthlyOverviewDto
                {
                    Month = m,
                    MonthName = monthNames[m - 1],
                    Year = targetYear,
                    TotalIncome = incomes.Where(i => i.CreatedAt.Month == m).Sum(i => (decimal)i.IncomeAmount),
                    TotalExpense = expenses.Where(e => e.CreatedAt.Month == m).Sum(e => (decimal)e.ExpenseAmount)
                })
                .ToList();

            return Ok(new AnalyticsSummaryDto
            {
                TotalIncome = totalIncome,
                TotalExpense = totalExpense,
                NetSavings = totalIncome - totalExpense,
                CategoryBreakdown = categoryBreakdown,
                MonthlyOverview = monthlyOverview
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error generating analytics", error = ex.Message });
        }
    }
}