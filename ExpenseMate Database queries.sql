create database ExpenseMate

create table Income(
incomeId int identity(1,1) primary key,
incomeAmount float not null,
category nvarchar(30) not null,
CreatedAt DATETIME2 DEFAULT GETUTCDATE()
)

select * from Income

create table Expense(
expenseId int identity(1,1) primary key,
expenseAmount float not null,
expenseCategory nvarchar(30) not null,
CreatedAt DATETIME2 DEFAULT GETUTCDATE()
)

create table Budget(
budgetId int identity(1,1) primary key,
budgetAmount float not null,
alertPercent float not null,
budgetMonth nvarchar(30) not null,
)

select * from Budget

use database ExpenseMate

alter table Budget add constraint chk_budget_alert check(alertPercent>=0 and alertPercent<=100)