using FinanceManager.Core.Models;
using FinanceManager.Core.Services;

namespace FinanceManager.Tests;

public class TransactionFilterTests
{
    private static List<Transaction> SampleTransactions() => new()
    {
        new() { Id = 1, Amount = 5000, Type = TransactionType.Income,  CategoryId = 1, Date = new DateTime(2026, 1, 5),  Comment = "Зарплата" },
        new() { Id = 2, Amount = 1200, Type = TransactionType.Expense, CategoryId = 2, Date = new DateTime(2026, 1, 15), Comment = "Продукты" },
        new() { Id = 3, Amount = 800,  Type = TransactionType.Expense, CategoryId = 3, Date = new DateTime(2026, 2, 3),  Comment = "Кофе" },
        new() { Id = 4, Amount = 3000, Type = TransactionType.Income,  CategoryId = 1, Date = new DateTime(2026, 2, 20), Comment = "Фриланс проект" },
        new() { Id = 5, Amount = 500,  Type = TransactionType.Expense, CategoryId = 2, Date = new DateTime(2026, 3, 10), Comment = "Продукты на неделю" }
    };

    [Fact]
    public void Filter_NoFilters_ReturnsAll()
    {
        var result = FinanceCalculator.Filter(SampleTransactions(), null, null, null, null);

        Assert.Equal(5, result.Count());
    }

    [Fact]
    public void Filter_ByDateFrom_ReturnsOnlyAfterDate()
    {
        var from = new DateTime(2026, 2, 1);

        var result = FinanceCalculator.Filter(SampleTransactions(), from, null, null, null);

        Assert.All(result, t => Assert.True(t.Date >= from));
        Assert.Equal(3, result.Count());
    }

    [Fact]
    public void Filter_ByDateRange_ReturnsOnlyWithinRange()
    {
        var from = new DateTime(2026, 1, 1);
        var to = new DateTime(2026, 1, 31);

        var result = FinanceCalculator.Filter(SampleTransactions(), from, to, null, null);

        Assert.Equal(2, result.Count());
        Assert.All(result, t => Assert.InRange(t.Date, from, to));
    }

    [Fact]
    public void Filter_ByCategoryId_ReturnsOnlyMatchingCategory()
    {
        var result = FinanceCalculator.Filter(SampleTransactions(), null, null, 2, null);

        Assert.Equal(2, result.Count());
        Assert.All(result, t => Assert.Equal(2, t.CategoryId));
    }

    [Fact]
    public void Filter_BySearch_ReturnsMatchingComments()
    {
        var result = FinanceCalculator.Filter(SampleTransactions(), null, null, null, "продукты");

        Assert.Equal(2, result.Count());
        Assert.All(result, t => Assert.Contains("родукты", t.Comment, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Filter_MultipleFilters_CombinesCorrectly()
    {
        var from = new DateTime(2026, 1, 1);
        var to = new DateTime(2026, 3, 31);

        var result = FinanceCalculator.Filter(SampleTransactions(), from, to, 2, null);

        Assert.Equal(2, result.Count());
        Assert.All(result, t =>
        {
            Assert.Equal(2, t.CategoryId);
            Assert.InRange(t.Date, from, to);
        });
    }

    [Fact]
    public void Filter_SearchNoMatch_ReturnsEmpty()
    {
        var result = FinanceCalculator.Filter(SampleTransactions(), null, null, null, "несуществующее");

        Assert.Empty(result);
    }
}
