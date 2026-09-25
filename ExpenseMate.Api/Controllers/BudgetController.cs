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
    public async Task<IActionResult> Create([FromBody] Budget budget)
    {
        try
        {
            await _budgetService.SetBudgetAsync(budget);
            return CreatedAtAction(nameof(GetByMonth), new { month = budget.BudgetMonth }, budget);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Budget budget)
    {
        if (id != budget.BudgetId)
            return BadRequest("Route ID does not match entity ID.");

        try
        {
            await _budgetService.UpdateBudgetAsync(budget);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _budgetService.DeleteBudgetAsync(id);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}