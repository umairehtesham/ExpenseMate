using ExpenseMate.Domain;
using ExpenseMate.Services;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseMate.Api.Controller;

[ApiController]
[Route("api/[controller]")]
public class IncomeController : ControllerBase
{
    private readonly IIncomeService _incomeService;

    public IncomeController(IIncomeService incomeService)
    {
        _incomeService=incomeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var income = await _incomeService.GetAllIncomeAsync();
        return Ok(income);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var income=await _incomeService.GetIncomeByIdAsync(id);
        if (income == null)
        {
            return NotFound($"Income with id {id} not found");
        }

        return Ok(income);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Income income)
    {
        try
        {
            await _incomeService.AddIncomeAsync(income);
            return CreatedAtAction(nameof(GetById), new { id = income.IncomeId }, income);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Income income)
    {
        if (id != income.IncomeId)
        {
            return BadRequest("Route ID does not match entity ID.");
        }

        try
        {
            await _incomeService.UpdateIncomeAsync(income);
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
            await _incomeService.DeleteIncomeAsync(id);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}