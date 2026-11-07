using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace ExpenseMate.Tests;

[TestFixture]
public class ExpenseMateE2ETests : PageTest
{
    private const string BaseUrl = "http://localhost:4200";

    // ------------------------------------------------------------------
    // Selectors that may need adjusting to match your real markup.
    // ------------------------------------------------------------------
    private const string RecordRowSelector = "tr, li, .list-group-item";
    private static readonly Regex EditButtonName = new("Edit|✏", RegexOptions.IgnoreCase);
    private static readonly Regex SaveEditButtonName = new("^(Save|Update)", RegexOptions.IgnoreCase);
    private static readonly Regex DeleteButtonName = new("Delete|🗑", RegexOptions.IgnoreCase);

    private static string CurrentMonth => DateTime.Now.ToString("MMMM", CultureInfo.InvariantCulture);

    public override BrowserNewContextOptions ContextOptions()
    {
        return new BrowserNewContextOptions
        {
            IgnoreHTTPSErrors = true
        };
    }

    [SetUp]
    public async Task Setup()
    {
        Page.SetDefaultTimeout(10000);

        // Auto-accept any confirm() dialog raised by Delete buttons
        Page.Dialog += async (_, dialog) => await dialog.AcceptAsync();

        await Page.GotoAsync(BaseUrl);
    }

    // ==================================================================
    // Helpers
    // ==================================================================

    private static string Unique(string prefix) => $"{prefix}-{DateTime.Now:HHmmssfff}";

    private static string Num(decimal value) => value.ToString("0.##", CultureInfo.InvariantCulture);

    private ILocator ErrorLocator =>
        Page.Locator(".error-message, .alert-danger, [role='alert'], .toast");

    private ILocator SuccessLocator =>
        Page.Locator(".alert-success, .toast-success, .success-message")
            .Or(Page.GetByText(new Regex("saved|success|updated", RegexOptions.IgnoreCase)));

    private ILocator RecordRow(string text) =>
        Page.Locator(RecordRowSelector).Filter(new() { HasText = text }).First;

    private async Task GoToIncomeAsync() =>
        await Page.GetByRole(AriaRole.Link, new() { NameRegex = new Regex("^Income$", RegexOptions.IgnoreCase) }).First.ClickAsync();

    private async Task GoToExpensesAsync() =>
        await Page.GetByRole(AriaRole.Link, new() { NameRegex = new Regex("^Expenses$", RegexOptions.IgnoreCase) }).First.ClickAsync();

    private async Task GoToBudgetsAsync() =>
        await Page.GetByRole(AriaRole.Link, new() { NameRegex = new Regex("^Budgets$", RegexOptions.IgnoreCase) }).First.ClickAsync();

    private async Task AddIncomeAsync(string category, string amount)
    {
        await Page.GetByPlaceholder("e.g. Primary Job, Freelance").FillAsync(category);
        await Page.Locator("input[type='number']").First.FillAsync(amount);
        await Page.GetByRole(AriaRole.Button, new() { Name = "+ Add Income" }).ClickAsync();
    }

    private async Task AddExpenseAsync(string category, string amount)
    {
        await Page.GetByPlaceholder("e.g. Food, Utilities, Rent").FillAsync(category);
        await Page.Locator("input[type='number']").First.FillAsync(amount);
        await Page.GetByRole(AriaRole.Button, new() { Name = "+ Add Expense" }).ClickAsync();
    }

    /// <summary>
    /// A submission counts as rejected if either the browser's native validation
    /// flagged an input, or the app displayed an error element.
    /// </summary>
    private async Task ExpectRejectedAsync(string because)
    {
        await Page.WaitForTimeoutAsync(500);
        var nativeInvalid = await Page.Locator("input:invalid").CountAsync();
        var appError = await ErrorLocator.First.IsVisibleAsync();
        Assert.That(nativeInvalid > 0 || appError, Is.True, because);
    }

    private async Task EditRecordAsync(string oldText, string newCategory)
    {
        var row = RecordRow(oldText);
        await row.GetByRole(AriaRole.Button, new() { NameRegex = EditButtonName }).First.ClickAsync();

        // Edit mode: the category text input is either inside the row or a form on the page
        var editInput = row.Locator("input[type='text']").Or(Page.Locator("input[type='text'][value]")).First;
        await editInput.FillAsync(newCategory);

        await Page.GetByRole(AriaRole.Button, new() { NameRegex = SaveEditButtonName }).First.ClickAsync();
    }

    private async Task DeleteRecordAsync(string text)
    {
        await RecordRow(text).GetByRole(AriaRole.Button, new() { NameRegex = DeleteButtonName }).First.ClickAsync();
    }

    /// <summary>Reads a KPI value (e.g. "Total Income") from the Analytics page.</summary>
    private async Task<decimal> ReadAnalyticsKpiAsync(string label)
    {
        await Page.GotoAsync($"{BaseUrl}/analytics");

        var retryBtn = Page.GetByRole(AriaRole.Button, new() { Name = "Retry" });
        if (await retryBtn.IsVisibleAsync())
        {
            await retryBtn.ClickAsync();
        }

        await Expect(Page.GetByText(label).First).ToBeVisibleAsync();

        var pattern = new Regex(Regex.Escape(label) + @"\s*\$?\s*(-?[\d,]+(?:\.\d+)?)");
        for (var i = 0; i < 20; i++)
        {
            var body = await Page.Locator("body").InnerTextAsync();
            var match = pattern.Match(body);
            if (match.Success)
            {
                return decimal.Parse(match.Groups[1].Value.Replace(",", ""), CultureInfo.InvariantCulture);
            }
            await Page.WaitForTimeoutAsync(500);
        }

        Assert.Fail($"Could not read a numeric value for '{label}' on the Analytics page.");
        return 0;
    }

    private async Task SaveBudgetAsync(string month, string amount, string thresholdPercent)
    {
        await GoToBudgetsAsync();
        await Page.Locator("select").First.SelectOptionAsync(new[] { month });

        var inputs = Page.Locator("input[type='number']");
        await inputs.Nth(0).FillAsync(amount);           // Budget Amount ($)
        await inputs.Nth(1).FillAsync(thresholdPercent); // Threshold (%)

        await Page.GetByRole(AriaRole.Button, new() { Name = "Save Budget" }).ClickAsync();
    }

    /// <summary>Clicks "Check Status" for the month and returns the "Spent:" amount.</summary>
    private async Task<decimal> CheckStatusAsync(string month)
    {
        await Page.Locator("select").First.SelectOptionAsync(new[] { month });
        await Page.GetByRole(AriaRole.Button, new() { Name = "Check Status" }).ClickAsync();
        await Expect(Page.GetByText("Spent:").First).ToBeVisibleAsync();

        var body = await Page.Locator("body").InnerTextAsync();
        var match = Regex.Match(body, @"Spent:\s*\$?\s*([\d,]+(?:\.\d+)?)");
        Assert.That(match.Success, Is.True, "Could not read the 'Spent:' value.");
        return decimal.Parse(match.Groups[1].Value.Replace(",", ""), CultureInfo.InvariantCulture);
    }

    private async Task ExpectNoAlertAsync() =>
        await Expect(Page.GetByText("Alert Triggered")).ToBeHiddenAsync();

    private async Task ExpectAlertAsync() =>
        await Expect(Page.GetByText("Alert Triggered")).ToBeVisibleAsync();

    // ==================================================================
    // Existing smoke tests (unchanged)
    // ==================================================================

    [Test]
    public async Task TC01_Verify_Dashboard_Header_And_Navigation_Cards()
    {
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Welcome to ExpenseMate" }))
            .ToBeVisibleAsync();

        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Income Management" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Expense Tracker" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Budget & Alerts" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Analytics & Visualization" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "CSV Data Management" })).ToBeVisibleAsync();
    }

    [Test]
    public async Task TC02_Navigate_To_Analytics_And_Verify_KPI_Cards()
    {
        await Page.GetByRole(AriaRole.Link, new() { NameRegex = new Regex("Analytics", RegexOptions.IgnoreCase) }).First.ClickAsync();

        await Expect(Page).ToHaveURLAsync(new Regex(".*/analytics"));

        var retryBtn = Page.GetByRole(AriaRole.Button, new() { Name = "Retry" });
        if (await retryBtn.IsVisibleAsync())
        {
            await retryBtn.ClickAsync();
        }

        await Expect(Page.GetByText("Total Income")).ToBeVisibleAsync();
        await Expect(Page.GetByText("Total Expense")).ToBeVisibleAsync();
        await Expect(Page.GetByText("Net Savings")).ToBeVisibleAsync();
    }

    [Test]
    public async Task TC03_Navigate_To_CSV_Page_And_Verify_Import_Export_Sections()
    {
        await Page.GetByRole(AriaRole.Link, new() { NameRegex = new Regex("CSV", RegexOptions.IgnoreCase) }).First.ClickAsync();

        await Expect(Page).ToHaveURLAsync(new Regex(".*/csv"));

        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Export All Data" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Export Incomes Only" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Export Expenses Only" })).ToBeVisibleAsync();

        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Upload & Import" })).ToBeVisibleAsync();
    }

    [Test]
    public async Task TC04_Verify_Year_Filter_Selection_On_Analytics()
    {
        await Page.GotoAsync($"{BaseUrl}/analytics");

        var yearSelect = Page.Locator("#year");

        await yearSelect.SelectOptionAsync(new[] { "2026" });

        await Expect(yearSelect.Locator("option:checked")).ToHaveTextAsync("2026");
    }

    [Test]
    public async Task TC05_Income_Negative_Amount_Validation()
    {
        await Page.GetByRole(AriaRole.Link, new() { NameRegex = new Regex("^Income$", RegexOptions.IgnoreCase) }).First.ClickAsync();

        await Page.GetByPlaceholder("e.g. Primary Job, Freelance").FillAsync("Invalid Bonus");

        var amountInput = Page.Locator("input[type='number']").First;
        await amountInput.FillAsync("-500");

        await Page.GetByRole(AriaRole.Button, new() { Name = "+ Add Income" }).ClickAsync();

        var errorOrToast = Page.Locator(".error-message, .alert-danger, [role='alert'], .toast");
        await Expect(errorOrToast.First).ToBeVisibleAsync();
    }

    [Test]
    public async Task TC06_Search_Non_Existent_Income_ID_Displays_Error()
    {
        await Page.GetByRole(AriaRole.Link, new() { NameRegex = new Regex("^Income$", RegexOptions.IgnoreCase) }).First.ClickAsync();

        await Page.GetByPlaceholder("Enter Income ID...").FillAsync("999999");

        await Page.GetByRole(AriaRole.Button, new() { Name = "Search" }).ClickAsync();

        var notFoundAlert = Page.GetByText(new Regex("not found|failed|error|no record", RegexOptions.IgnoreCase));
        await Expect(notFoundAlert.First).ToBeVisibleAsync();
    }

    [Test]
    public async Task TC07_Expense_Negative_Amount_Validation()
    {
        await Page.GetByRole(AriaRole.Link, new() { NameRegex = new Regex("^Expenses$", RegexOptions.IgnoreCase) }).First.ClickAsync();

        await Page.GetByPlaceholder("e.g. Food, Utilities, Rent").FillAsync("Negative Groceries");

        var amountInput = Page.Locator("input[type='number']").First;
        await amountInput.FillAsync("-150");

        await Page.GetByRole(AriaRole.Button, new() { Name = "+ Add Expense" }).ClickAsync();

        var errorOrToast = Page.Locator(".error-message, .alert-danger, [role='alert'], .toast");
        await Expect(errorOrToast.First).ToBeVisibleAsync();
    }

    [Test]
    public async Task TC08_Search_Non_Existent_Expense_ID_Displays_Error()
    {
        await Page.GetByRole(AriaRole.Link, new() { NameRegex = new Regex("^Expenses$", RegexOptions.IgnoreCase) }).First.ClickAsync();

        await Page.GetByPlaceholder("Enter Expense ID...").FillAsync("888888");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Search" }).ClickAsync();

        var notFoundAlert = Page.GetByText(new Regex("not found|failed|error|no record", RegexOptions.IgnoreCase));
        await Expect(notFoundAlert.First).ToBeVisibleAsync();
    }

    [Test]
    public async Task TC09_Budget_Negative_Or_Invalid_Amount_Cannot_Be_Saved()
    {
        await Page.GetByRole(AriaRole.Link, new() { NameRegex = new Regex("^Budgets$", RegexOptions.IgnoreCase) }).First.ClickAsync();

        var budgetInputs = Page.Locator("input[type='number']");
        await budgetInputs.Nth(0).FillAsync("-1000"); // Budget Amount ($)
        await budgetInputs.Nth(1).FillAsync("80");    // Threshold (%)

        await Page.GetByRole(AriaRole.Button, new() { Name = "Save Budget" }).ClickAsync();

        var errorOrToast = Page.Locator(".error-message, .alert-danger, [role='alert'], .toast");
        await Expect(errorOrToast.First).ToBeVisibleAsync();
    }

    [Test]
    public async Task TC10_Check_Monthly_Budget_Alert_Triggered()
    {
        await Page.GetByRole(AriaRole.Link, new() { NameRegex = new Regex("^Budgets$", RegexOptions.IgnoreCase) }).First.ClickAsync();

        var monthSelect = Page.Locator("select").First;
        await monthSelect.SelectOptionAsync(new[] { "September" });

        await Page.GetByRole(AriaRole.Button, new() { Name = "Check Status" }).ClickAsync();

        await Expect(Page.GetByText("Alert Triggered")).ToBeVisibleAsync();
        await Expect(Page.GetByText("Spent:")).ToBeVisibleAsync();
        await Expect(Page.GetByText("Alert Threshold:")).ToBeVisibleAsync();
    }

    // ==================================================================
    // 1. Income test cases
    // (INC-03 negative amount is already covered by TC05)
    // ==================================================================

    [Test]
    public async Task INC01_Add_Income_With_Valid_Details_Is_Saved()
    {
        await GoToIncomeAsync();
        var category = Unique("Salary");

        await AddIncomeAsync(category, "2500");

        await Expect(RecordRow(category)).ToBeVisibleAsync();
    }

    [Test]
    public async Task INC02_Add_Income_With_Missing_Required_Fields_Shows_Validation_Error()
    {
        await GoToIncomeAsync();

        // Both fields left empty
        await Page.GetByRole(AriaRole.Button, new() { Name = "+ Add Income" }).ClickAsync();
        await ExpectRejectedAsync("Submitting an empty income form should be rejected.");

        // Only category filled, amount missing
        var category = Unique("NoAmount");
        await Page.GetByPlaceholder("e.g. Primary Job, Freelance").FillAsync(category);
        await Page.GetByRole(AriaRole.Button, new() { Name = "+ Add Income" }).ClickAsync();
        await ExpectRejectedAsync("Income without an amount should be rejected.");

        await Expect(Page.Locator(RecordRowSelector).Filter(new() { HasText = category })).ToHaveCountAsync(0);
    }

    [Test]
    public async Task INC04_Add_Income_With_Zero_Amount_Is_Rejected()
    {
        await GoToIncomeAsync();
        var category = Unique("ZeroIncome");

        await AddIncomeAsync(category, "0");

        // Assumes zero is not allowed (the expense form uses min="1").
        // If your rules allow zero, change this to assert the row is visible instead.
        await ExpectRejectedAsync("A zero income amount should be rejected.");
        await Expect(Page.Locator(RecordRowSelector).Filter(new() { HasText = category })).ToHaveCountAsync(0);
    }

    [Test]
    public async Task INC05_View_Income_Records_Shows_All_Saved_Entries()
    {
        await GoToIncomeAsync();
        var first = Unique("ViewA");
        var second = Unique("ViewB");

        await AddIncomeAsync(first, "100");
        await Expect(RecordRow(first)).ToBeVisibleAsync();
        await AddIncomeAsync(second, "200");
        await Expect(RecordRow(second)).ToBeVisibleAsync();

        // Both entries are still listed together
        await Expect(RecordRow(first)).ToBeVisibleAsync();
        await Expect(RecordRow(second)).ToBeVisibleAsync();
    }

    [Test]
    public async Task INC06_Edit_Income_Record_Saves_Updated_Information()
    {
        await GoToIncomeAsync();
        var original = Unique("EditMe");
        var updated = Unique("Edited");

        await AddIncomeAsync(original, "300");
        await Expect(RecordRow(original)).ToBeVisibleAsync();

        await EditRecordAsync(original, updated);

        await Expect(RecordRow(updated)).ToBeVisibleAsync();
        await Expect(Page.Locator(RecordRowSelector).Filter(new() { HasText = original })).ToHaveCountAsync(0);
    }

    [Test]
    public async Task INC07_Delete_Income_Record_Removes_It()
    {
        await GoToIncomeAsync();
        var category = Unique("DeleteMe");

        await AddIncomeAsync(category, "150");
        await Expect(RecordRow(category)).ToBeVisibleAsync();

        await DeleteRecordAsync(category);

        await Expect(Page.Locator(RecordRowSelector).Filter(new() { HasText = category })).ToHaveCountAsync(0);
    }

    [Test]
    public async Task INC08_Total_Income_Matches_Saved_Records()
    {
        var before = await ReadAnalyticsKpiAsync("Total Income");

        await Page.GotoAsync(BaseUrl);
        await GoToIncomeAsync();
        await AddIncomeAsync(Unique("TotalCheck"), "123");
        await Expect(RecordRow("TotalCheck")).ToBeVisibleAsync();

        var after = await ReadAnalyticsKpiAsync("Total Income");

        Assert.That(after - before, Is.EqualTo(123m), "Total Income should increase by the amount just added.");
    }

    // ==================================================================
    // 2. Expense test cases
    // (EXP-03 negative amount is already covered by TC07)
    // ==================================================================

    [Test]
    public async Task EXP01_Add_Expense_With_Valid_Details_Is_Saved()
    {
        await GoToExpensesAsync();
        var category = Unique("Groceries");

        await AddExpenseAsync(category, "75");

        await Expect(RecordRow(category)).ToBeVisibleAsync();
    }

    [Test]
    public async Task EXP02_Add_Expense_With_Missing_Required_Fields_Shows_Validation_Error()
    {
        await GoToExpensesAsync();

        await Page.GetByRole(AriaRole.Button, new() { Name = "+ Add Expense" }).ClickAsync();
        await ExpectRejectedAsync("Submitting an empty expense form should be rejected.");

        var category = Unique("NoAmount");
        await Page.GetByPlaceholder("e.g. Food, Utilities, Rent").FillAsync(category);
        await Page.GetByRole(AriaRole.Button, new() { Name = "+ Add Expense" }).ClickAsync();
        await ExpectRejectedAsync("Expense without an amount should be rejected.");

        await Expect(Page.Locator(RecordRowSelector).Filter(new() { HasText = category })).ToHaveCountAsync(0);
    }

    [Test]
    public async Task EXP04_Selected_Expense_Category_Is_Saved_Correctly()
    {
        await GoToExpensesAsync();
        var category = Unique("Utilities");

        await AddExpenseAsync(category, "60");

        var row = RecordRow(category);
        await Expect(row).ToBeVisibleAsync();
        await Expect(row).ToContainTextAsync(category);
    }

    [Test]
    public async Task EXP05_View_Expense_Records_Shows_All_Saved_Entries()
    {
        await GoToExpensesAsync();
        var first = Unique("ViewA");
        var second = Unique("ViewB");

        await AddExpenseAsync(first, "10");
        await Expect(RecordRow(first)).ToBeVisibleAsync();
        await AddExpenseAsync(second, "20");
        await Expect(RecordRow(second)).ToBeVisibleAsync();

        await Expect(RecordRow(first)).ToBeVisibleAsync();
        await Expect(RecordRow(second)).ToBeVisibleAsync();
    }

    [Test]
    public async Task EXP06_Edit_Expense_Record_Saves_Updated_Information()
    {
        await GoToExpensesAsync();
        var original = Unique("EditMe");
        var updated = Unique("Edited");

        await AddExpenseAsync(original, "45");
        await Expect(RecordRow(original)).ToBeVisibleAsync();

        await EditRecordAsync(original, updated);

        await Expect(RecordRow(updated)).ToBeVisibleAsync();
        await Expect(Page.Locator(RecordRowSelector).Filter(new() { HasText = original })).ToHaveCountAsync(0);
    }

    [Test]
    public async Task EXP07_Delete_Expense_Record_Removes_It()
    {
        await GoToExpensesAsync();
        var category = Unique("DeleteMe");

        await AddExpenseAsync(category, "35");
        await Expect(RecordRow(category)).ToBeVisibleAsync();

        await DeleteRecordAsync(category);

        await Expect(Page.Locator(RecordRowSelector).Filter(new() { HasText = category })).ToHaveCountAsync(0);
    }

    [Test]
    public async Task EXP08_Total_Expenses_Matches_Saved_Records()
    {
        var before = await ReadAnalyticsKpiAsync("Total Expense");

        await Page.GotoAsync(BaseUrl);
        await GoToExpensesAsync();
        await AddExpenseAsync(Unique("TotalCheck"), "88");
        await Expect(RecordRow("TotalCheck")).ToBeVisibleAsync();

        var after = await ReadAnalyticsKpiAsync("Total Expense");

        Assert.That(after - before, Is.EqualTo(88m), "Total Expense should increase by the amount just added.");
    }

    [Test]
    public async Task EXP09_Add_Expense_With_Future_Date_Follows_Validation_Rules()
    {
        await GoToExpensesAsync();

        var dateInput = Page.Locator("input[type='date']");
        if (await dateInput.CountAsync() == 0)
        {
            Assert.Ignore("The Expense form has no date field (CreatedAt is set automatically), so this case is not applicable yet.");
        }

        var category = Unique("FutureDate");
        await dateInput.First.FillAsync(DateTime.Now.AddYears(1).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
        await AddExpenseAsync(category, "50");

        // The app must handle it one way or the other: reject with a message, or save and list it.
        var saved = RecordRow(category);
        await Page.WaitForTimeoutAsync(1000);
        var wasSaved = await saved.IsVisibleAsync();
        var wasRejected = await ErrorLocator.First.IsVisibleAsync() || await Page.Locator("input:invalid").CountAsync() > 0;

        Assert.That(wasSaved || wasRejected, Is.True,
            "A future-dated expense must either be saved or rejected with feedback.");
    }

    // ==================================================================
    // 3. Budget test cases
    // (negative budget AMOUNT is covered by TC09; BUD-02 below covers the percentage)
    // ==================================================================

    [Test]
    public async Task BUD01_Set_Valid_Budget_Percentage_Is_Saved()
    {
        await SaveBudgetAsync(CurrentMonth, "5000", "80");

        await Expect(SuccessLocator.First).ToBeVisibleAsync();
    }

    [Test]
    public async Task BUD02_Negative_Budget_Percentage_Is_Rejected()
    {
        await SaveBudgetAsync(CurrentMonth, "5000", "-10");

        await ExpectRejectedAsync("A negative budget percentage should be rejected.");
    }

    [Test]
    public async Task BUD03_Budget_Percentage_Above_100_Is_Rejected()
    {
        await SaveBudgetAsync(CurrentMonth, "5000", "150");

        await ExpectRejectedAsync("A budget percentage above 100 should be rejected.");
    }

    [Test]
    public async Task BUD04_Budget_Percentage_Zero_Boundary_Is_Handled()
    {
        await SaveBudgetAsync(CurrentMonth, "5000", "0");

        // Either outcome is valid per the rules, but the app must give clear feedback.
        await Expect(SuccessLocator.Or(ErrorLocator).First).ToBeVisibleAsync();
    }

    [Test]
    public async Task BUD05_Budget_Percentage_100_Maximum_Is_Accepted()
    {
        await SaveBudgetAsync(CurrentMonth, "5000", "100");

        await Expect(SuccessLocator.First).ToBeVisibleAsync();
    }

    [Test]
    public async Task BUD06_Spending_Below_Threshold_Shows_No_Warning()
    {
        // Huge budget so existing spending is far below the threshold
        await SaveBudgetAsync(CurrentMonth, "100000000", "80");

        await CheckStatusAsync(CurrentMonth);

        await ExpectNoAlertAsync();
    }

    [Test]
    public async Task BUD07_Spending_Reaching_Threshold_Shows_Warning()
    {
        // Make sure there is some spending in the month
        await GoToExpensesAsync();
        await AddExpenseAsync(Unique("Threshold"), "100");
        await Expect(RecordRow("Threshold")).ToBeVisibleAsync();

        await SaveBudgetAsync(CurrentMonth, "100000000", "80");
        var spent = await CheckStatusAsync(CurrentMonth);

        // Budget chosen so that the 50% threshold equals the current spending exactly
        await SaveBudgetAsync(CurrentMonth, Num(spent * 2), "50");
        await CheckStatusAsync(CurrentMonth);

        await ExpectAlertAsync();
    }

    [Test]
    public async Task BUD08_Spending_Exceeding_Threshold_Shows_Alert()
    {
        await GoToExpensesAsync();
        await AddExpenseAsync(Unique("Exceed"), "100");
        await Expect(RecordRow("Exceed")).ToBeVisibleAsync();

        await SaveBudgetAsync(CurrentMonth, "100000000", "80");
        var spent = await CheckStatusAsync(CurrentMonth);

        // Budget of half the spending with a 100% threshold: spending is 200% of the budget
        await SaveBudgetAsync(CurrentMonth, Num(spent / 2), "100");
        await CheckStatusAsync(CurrentMonth);

        await ExpectAlertAsync();
        await Expect(Page.GetByText("Alert Threshold:")).ToBeVisibleAsync();
    }

    [Test]
    public async Task BUD09_Adding_Expense_That_Crosses_Threshold_Triggers_Alert()
    {
        await SaveBudgetAsync(CurrentMonth, "100000000", "100");
        var spentBefore = await CheckStatusAsync(CurrentMonth);

        // Budget leaves 100 of headroom, so there is no alert yet
        await SaveBudgetAsync(CurrentMonth, Num(spentBefore + 100), "100");
        await CheckStatusAsync(CurrentMonth);
        await ExpectNoAlertAsync();

        // Add an expense of 150, which crosses the threshold
        await GoToExpensesAsync();
        await AddExpenseAsync(Unique("Cross"), "150");
        await Expect(RecordRow("Cross")).ToBeVisibleAsync();

        await GoToBudgetsAsync();
        await CheckStatusAsync(CurrentMonth);

        await ExpectAlertAsync();
    }

    [Test]
    public async Task BUD10_Deleting_Expense_Below_Threshold_Clears_Alert()
    {
        await SaveBudgetAsync(CurrentMonth, "100000000", "100");
        var spentBefore = await CheckStatusAsync(CurrentMonth);

        await SaveBudgetAsync(CurrentMonth, Num(spentBefore + 100), "100");

        var category = Unique("Spike");
        await GoToExpensesAsync();
        await AddExpenseAsync(category, "150");
        await Expect(RecordRow(category)).ToBeVisibleAsync();

        await GoToBudgetsAsync();
        await CheckStatusAsync(CurrentMonth);
        await ExpectAlertAsync();

        // Remove the expense that caused the alert
        await GoToExpensesAsync();
        await DeleteRecordAsync(category);
        await Expect(Page.Locator(RecordRowSelector).Filter(new() { HasText = category })).ToHaveCountAsync(0);

        await GoToBudgetsAsync();
        await CheckStatusAsync(CurrentMonth);

        await ExpectNoAlertAsync();
    }

    [Test]
    public async Task BUD11_Changing_Budget_Percentage_Updates_Warning_Threshold()
    {
        await GoToExpensesAsync();
        await AddExpenseAsync(Unique("Change"), "100");
        await Expect(RecordRow("Change")).ToBeVisibleAsync();

        await SaveBudgetAsync(CurrentMonth, "100000000", "100");
        var spent = await CheckStatusAsync(CurrentMonth);

        // Budget is twice the spending; 100% threshold means no alert
        var budget = spent * 2;
        await SaveBudgetAsync(CurrentMonth, Num(budget), "100");
        await CheckStatusAsync(CurrentMonth);
        await ExpectNoAlertAsync();

        // Lowering the threshold to 40% puts it below the current spending
        await SaveBudgetAsync(CurrentMonth, Num(budget), "40");
        await CheckStatusAsync(CurrentMonth);

        await ExpectAlertAsync();
    }

    [Test]
    public async Task BUD12_Budget_Percentage_Calculation_Is_Accurate()
    {
        await GoToExpensesAsync();
        await AddExpenseAsync(Unique("Calc"), "100");
        await Expect(RecordRow("Calc")).ToBeVisibleAsync();

        await SaveBudgetAsync(CurrentMonth, "100000000", "80");
        var spent = await CheckStatusAsync(CurrentMonth);

        // Budget is 4x the spending, so spending should be displayed as 25%
        await SaveBudgetAsync(CurrentMonth, Num(spent * 4), "80");
        await CheckStatusAsync(CurrentMonth);

        // Adjust the format below if your UI shows the percentage differently
        await Expect(Page.GetByText(new Regex(@"\b25(\.0+)?\s*%"))).ToBeVisibleAsync();
    }

    [Test]
    public async Task BUD13_Refresh_After_Budget_Warning_Keeps_Alert_State_Consistent()
    {
        await GoToExpensesAsync();
        await AddExpenseAsync(Unique("Refresh"), "100");
        await Expect(RecordRow("Refresh")).ToBeVisibleAsync();

        await SaveBudgetAsync(CurrentMonth, "100000000", "80");
        var spent = await CheckStatusAsync(CurrentMonth);

        await SaveBudgetAsync(CurrentMonth, Num(spent / 2), "100");
        await CheckStatusAsync(CurrentMonth);
        await ExpectAlertAsync();

        await Page.ReloadAsync();
        await GoToBudgetsAsync();
        await CheckStatusAsync(CurrentMonth);

        await ExpectAlertAsync();
        await Expect(Page.GetByText("Alert Threshold:")).ToBeVisibleAsync();
    }
}