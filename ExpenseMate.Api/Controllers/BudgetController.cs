using ExpenseMate.Domain;
using ExpenseMate.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseMate.Api.Controller;

[ApiController]
[Route("api/[controller]")]
public class BudgetController : ControllerBase
{
    private readonly IBudgetService _budgetService;

    public BudgetController(IBudgetService budgetService)
    {
        _budgetService = budgetService;
    }

    [HttpGet("{month}")]
    public async Task<IActionResult> GetByMonth(string month)
    {
        var budget = await _budgetService.GetBudgetByMonthAsync(month);
        if (budget == null)
            return NotFound($"No budget found for month {month}.");

        return Ok(budget);
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrUpdateBudget([FromBody] Budget budget)
    {
        // 1. Validate payload and month
        if (budget == null || string.IsNullOrWhiteSpace(budget.BudgetMonth))
        {
            return BadRequest(new { message = "Invalid budget payload or month is required." });
        }

        // 2. Validate non-negative / non-zero budget amount
        if (budget.BudgetAmount <= 0)
        {
            return BadRequest(new { message = "Budget amount must be greater than zero." });
        }

        // 3. Validate threshold percentage range (1% to 100%)
        if (budget.AlertPercent <= 0 || budget.AlertPercent > 100)
        {
            return BadRequest(new { message = "Alert threshold percentage must be between 1 and 100." });
        }

        var result = await _budgetService.SaveBudgetAsync(budget);
        return Ok(result);
    }
}