using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseMate.Database.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Budget",
                columns: table => new
                {
                    BudgetId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BudgetAmount = table.Column<float>(type: "real", nullable: false),
                    AlertPercent = table.Column<float>(type: "real", nullable: false),
                    BudgetMonth = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Budget", x => x.BudgetId);
                });

            migrationBuilder.CreateTable(
                name: "Expense",
                columns: table => new
                {
                    ExpenseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExpenseAmount = table.Column<float>(type: "real", nullable: false),
                    ExpenseCategory = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expense", x => x.ExpenseId);
                });

            migrationBuilder.CreateTable(
                name: "Income",
                columns: table => new
                {
                    IncomeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IncomeAmount = table.Column<float>(type: "real", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Income", x => x.IncomeId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Budget");

            migrationBuilder.DropTable(
                name: "Expense");

            migrationBuilder.DropTable(
                name: "Income");
        }
    }
}
