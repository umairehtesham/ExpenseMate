using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpenseMate.Database;
using ExpenseMate.Domain;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Globalization;

namespace ExpenseMate.Api.Controllers;

public class CsvImportResponse
{
    public string Message { get; set; } = string.Empty;
    public int InsertedCount { get; set; }
    public int IncomesInserted { get; set; }
    public int ExpensesInserted { get; set; }
    public int SkippedRows { get; set; }
    public bool IsSuccess { get; set; }
}

[ApiController]
[Route("api/[controller]")]
public class CsvController : ControllerBase
{
    private readonly ExpenseMateDbContext _context;

    public CsvController(ExpenseMateDbContext context)
    {
        _context = context;
    }

    [HttpGet("export/{type}")]
    public async Task<IActionResult> ExportCsv(string type)
    {
        var builder = new StringBuilder();
        builder.AppendLine("Type,Amount,Category,Date");

        string normalizedType = type.ToLowerInvariant();

        if (normalizedType == "all" || normalizedType == "incomes")
        {
            var incomes = await _context.Incomes.AsNoTracking().ToListAsync();
            foreach (var item in incomes)
            {
                builder.AppendLine($"Income,{item.IncomeAmount},\"{item.Category}\",{item.CreatedAt:yyyy-MM-dd}");
            }
        }

        if (normalizedType == "all" || normalizedType == "expenses")
        {
            var expenses = await _context.Expenses.AsNoTracking().ToListAsync();
            foreach (var item in expenses)
            {
                builder.AppendLine($"Expense,{item.ExpenseAmount},\"{item.ExpenseCategory}\",{item.CreatedAt:yyyy-MM-dd}");
            }
        }

        byte[] buffer = Encoding.UTF8.GetBytes(builder.ToString());
        return File(buffer, "text/csv", $"ExpenseMate_{type}_{DateTime.UtcNow:yyyyMMdd}.csv");
    }

    [HttpPost("import")]
    public async Task<ActionResult<CsvImportResponse>> ImportCsv(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new CsvImportResponse { IsSuccess = false, Message = "Please select a valid CSV file." });
        }

        int incomesInserted = 0;
        int expensesInserted = 0;
        int skippedRows = 0;

        using (var reader = new StreamReader(file.OpenReadStream()))
        {
            string? headerLine = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(headerLine))
            {
                return BadRequest(new CsvImportResponse { IsSuccess = false, Message = "CSV file is empty." });
            }

            while (!reader.EndOfStream)
            {
                string? line = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(line)) continue;

                string[] columns = line.Split(',');
                if (columns.Length < 4)
                {
                    skippedRows++;
                    continue;
                }

                string rowType = columns[0].Trim().ToLowerInvariant();
                string amountStr = columns[1].Trim();
                string category = columns[2].Trim().Trim('"');
                string dateStr = columns[3].Trim();

                if (!decimal.TryParse(amountStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal amount) ||
                    !DateTime.TryParse(dateStr, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                {
                    skippedRows++;
                    continue;
                }

                if (rowType.Contains("income"))
                {
                    _context.Incomes.Add(new Income
                    {
                        IncomeAmount = (float)amount,
                        Category = string.IsNullOrWhiteSpace(category) ? "General Income" : category,
                        CreatedAt = date
                    });
                    incomesInserted++;
                }
                else
                {
                    _context.Expenses.Add(new Expense
                    {
                        ExpenseAmount = (float)amount,
                        ExpenseCategory = string.IsNullOrWhiteSpace(category) ? "Uncategorized" : category,
                        CreatedAt = date
                    });
                    expensesInserted++;
                }
            }
        }

        await _context.SaveChangesAsync();

        return Ok(new CsvImportResponse
        {
            IsSuccess = true,
            Message = "CSV file parsed and imported successfully.",
            IncomesInserted = incomesInserted,
            ExpensesInserted = expensesInserted,
            InsertedCount = incomesInserted + expensesInserted,
            SkippedRows = skippedRows
        });
    }
}