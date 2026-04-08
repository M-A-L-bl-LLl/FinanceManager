using FinanceManager.Core.Services;

namespace FinanceManager.Tests;

public class BudgetStatusTests
{
    [Theory]
    [InlineData(0,      1000, BudgetStatusCode.Normal)]
    [InlineData(500,    1000, BudgetStatusCode.Normal)]
    [InlineData(799,    1000, BudgetStatusCode.Normal)]
    [InlineData(800,    1000, BudgetStatusCode.Warning)]
    [InlineData(950,    1000, BudgetStatusCode.Warning)]
    [InlineData(999,    1000, BudgetStatusCode.Warning)]
    [InlineData(1000,   1000, BudgetStatusCode.Over)]
    [InlineData(1500,   1000, BudgetStatusCode.Over)]
    public void GetBudgetStatus_VariousSpent_ReturnsExpectedStatus(
        decimal spent, decimal limit, BudgetStatusCode expected)
    {
        var status = FinanceCalculator.GetBudgetStatus(spent, limit);

        Assert.Equal(expected, status);
    }

    [Fact]
    public void GetBudgetStatus_ZeroLimit_ReturnsNormal()
    {
        var status = FinanceCalculator.GetBudgetStatus(9999, 0);

        Assert.Equal(BudgetStatusCode.Normal, status);
    }

    [Fact]
    public void GetBudgetStatus_NegativeLimit_ReturnsNormal()
    {
        var status = FinanceCalculator.GetBudgetStatus(100, -500);

        Assert.Equal(BudgetStatusCode.Normal, status);
    }

    [Fact]
    public void GetBudgetStatus_ExactlyEightyPercent_ReturnsWarning()
    {
        var status = FinanceCalculator.GetBudgetStatus(8_000, 10_000);

        Assert.Equal(BudgetStatusCode.Warning, status);
    }

    [Fact]
    public void GetBudgetStatus_ExactlyHundredPercent_ReturnsOver()
    {
        var status = FinanceCalculator.GetBudgetStatus(5_000, 5_000);

        Assert.Equal(BudgetStatusCode.Over, status);
    }
}
