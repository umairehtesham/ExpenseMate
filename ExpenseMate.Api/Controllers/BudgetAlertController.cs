using Microsoft.AspNetCore.Mvc;
using ExpenseMate.Services;
using System.Threading.Tasks;

namespace ExpenseMate.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Maps to "api/budgetalert"
    public class BudgetAlertController : ControllerBase
    {
        private readonly IBudgetAlertService _budgetAlertService;

        public BudgetAlertController(IBudgetAlertService budgetAlertService)
        {
            _budgetAlertService = budgetAlertService;
        }

        // Maps to GET api/budgetalert/{month}
        [HttpGet("{month}")]
        public async Task<IActionResult> CheckAlert(string month)
        {
            var alert = await _budgetAlertService.CheckBudgetAlertAsync(month);
            return Ok(alert);
        }
    }
}