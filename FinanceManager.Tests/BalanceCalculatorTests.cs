using FinanceManager.Core.Interfaces;
using FinanceManager.Core.Models;
using FinanceManager.Core.Services;
using Moq;

namespace FinanceManager.Tests;

public class BalanceCalculatorTests
{
    [Fact]
    public void CalculateBalance_MixedTransactions_ReturnsNetBalance()
    {
        var transactions = new List<Transaction>
        {
            new() { Amount = 10_000, Type = TransactionType.Income, Comment = "" },
            new() { Amount = 3_000,  Type = TransactionType.Expense, Comment = "" },
            new() { Amount = 1_500,  Type = TransactionType.Expense, Comment = "" }
        };

        var result = FinanceCalculator.CalculateBalance(transactions);

        Assert.Equal(5_500, result);
    }

    [Fact]
    public void CalculateBalance_NoTransactions_ReturnsZero()
    {
        var result = FinanceCalculator.CalculateBalance(Enumerable.Empty<Transaction>());

        Assert.Equal(0, result);
    }

    [Fact]
    public void CalculateBalance_OnlyExpenses_ReturnsNegative()
    {
        var transactions = new List<Transaction>
        {
            new() { Amount = 4_000, Type = TransactionType.Expense, Comment = "" },
            new() { Amount = 1_000, Type = TransactionType.Expense, Comment = "" }
        };

        var result = FinanceCalculator.CalculateBalance(transactions);

        Assert.Equal(-5_000, result);
    }

    [Fact]
    public async Task GetByPeriodAsync_MockedRepo_ReturnsTransactionsForPeriod()
    {
        var from = new DateTime(2026, 1, 1);
        var to = new DateTime(2026, 1, 31);

        var expected = new List<Transaction>
        {
            new() { Amount = 5_000, Type = TransactionType.Income, Date = new DateTime(2026, 1, 10), Comment = "" },
            new() { Amount = 2_000, Type = TransactionType.Expense, Date = new DateTime(2026, 1, 20), Comment = "" }
        };

        var mockRepo = new Mock<ITransactionRepository>();
        mockRepo.Setup(r => r.GetByPeriodAsync(from, to))
                .ReturnsAsync(expected);

        var transactions = await mockRepo.Object.GetByPeriodAsync(from, to);
        var balance = FinanceCalculator.CalculateBalance(transactions);

        mockRepo.Verify(r => r.GetByPeriodAsync(from, to), Times.Once);
        Assert.Equal(3_000, balance);
    }

    [Fact]
    public async Task AddAsync_MockedRepo_IsCalledOnce()
    {
        var transaction = new Transaction
        {
            Amount = 1_000,
            Type = TransactionType.Income,
            Date = DateTime.Today,
            Comment = "",
            CategoryId = 1
        };

        var mockRepo = new Mock<ITransactionRepository>();
        mockRepo.Setup(r => r.AddAsync(transaction)).Returns(Task.CompletedTask);

        await mockRepo.Object.AddAsync(transaction);

        mockRepo.Verify(r => r.AddAsync(transaction), Times.Once);
    }
}
