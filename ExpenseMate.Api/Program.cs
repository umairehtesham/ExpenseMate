using Microsoft.EntityFrameworkCore;
using ExpenseMate.Database;
using ExpenseMate.Domain;
using ExpenseMate.Services;
using ExpenseMate.Database;
using BudgetMate.Database;
var builder = WebApplication.CreateBuilder(args);

// 1. Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// 2. Database Context Registration
builder.Services.AddDbContext<ExpenseMateDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3. Register Repositories
builder.Services.AddScoped<IIncomeRepo, IncomeRepo>();
builder.Services.AddScoped<IExpenseRepo, ExpenseRepo>();
builder.Services.AddScoped<IBudgetRepo,BudgetRepo>();

// 4. Register Domain Services
builder.Services.AddScoped<IIncomeService, IncomeService>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<IBudgetService, BudgetService>();
builder.Services.AddScoped<IBudgetAlertService, BudgetAlertService>();

// 5. Controllers & OpenAPI / Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Enable Swagger in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// CRITICAL FIX: CORS middleware must execute before authorization and routing
app.UseCors("AllowAll");

// Disabled HTTPS Redirection so local HTTP requests to port 5001 don't hang on 307 redirects
// app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();

app.Run();