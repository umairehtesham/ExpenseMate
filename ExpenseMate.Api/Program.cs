using ExpenseMate.Domain;
using ExpenseMate.Services;
using ExpenseMate.Database;
using Microsoft.EntityFrameworkCore;
using BudgetMate.Database;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<ExpenseMateDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IIncomeRepo,IncomeRepo>();
builder.Services.AddScoped<IExpenseRepo,ExpenseRepo>();
builder.Services.AddScoped<IBudgetRepo,BudgetRepo>();
builder.Services.AddScoped<IBudgetAlertService, BudgetAlertService>();
builder.Services.AddScoped<IIncomeService,IncomeService>();
builder.Services.AddScoped<IExpenseService,ExpenseService>();
builder.Services.AddScoped<IBudgetService,BudgetService>();
builder.Services.AddScoped<IBudgetAlertService, BudgetAlertService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app=builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();