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
    if (budget == null || string.IsNullOrWhiteSpace(budget.BudgetMonth))
    {
        return BadRequest("Invalid budget payload.");
    }

    var result = await _budgetService.SaveBudgetAsync(budget);
    return Ok(result);
}

}