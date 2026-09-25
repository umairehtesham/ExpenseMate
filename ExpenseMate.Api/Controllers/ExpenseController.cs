using ExpenseMate.Domain;
using ExpenseMate.Services;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace ExpenseMate.Api.Controller;

[ApiController]
[Route("api/[controller]")]

public class ExpenseController : ControllerBase
{
    private readonly IExpenseService _expenseService;

    public ExpenseController(IExpenseService expenseService)
    {
        _expenseService=expenseService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var expenses=await _expenseService.GetAllExpensesAsync();
        return Ok(expenses);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
       var expense= await _expenseService.GetExpenseByIdAsync(id);
       if (expense == null)
        {
            return NotFound($"Expense with id {id} not found");
        }
        return Ok(expense);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Expense expense)
    {
        try
        {
            await _expenseService.AddExpenseAsync(expense);
            return CreatedAtAction(nameof(GetById), new { id = expense.ExpenseId }, expense);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id,Expense expense)
    {
        if (id == expense.ExpenseId)
        {
            return BadRequest("Route ID does not match entity ID.");
        }
        try
        {
            await _expenseService.UpdateExpenseAsync(expense);
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
            await _expenseService.DeleteExpenseAsync(id);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

}