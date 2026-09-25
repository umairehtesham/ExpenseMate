using ExpenseMate.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseMate.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BudgetAlertsController : ControllerBase
{
    private readonly IBudgetAlertService _budgetAlertService;

    public BudgetAlertsController(IBudgetAlertService budgetAlertService)
    {
        _budgetAlertService = budgetAlertService;
    }

    [HttpGet("{month}")]
    public async Task<IActionResult> CheckAlert(string month)
    {
        var result = await _budgetAlertService.CheckBudgetAlertAsync(month);
        return Ok(result);
    }
}