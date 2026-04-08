using FinanceManager.Core.Models;

namespace FinanceManager.Core.Services;

public enum BudgetStatusCode { Normal, Warning, Over }

public static class FinanceCalculator
{
    public static decimal CalculateBalance(IEnumerable<Transaction> transactions)
        => transactions.Sum(t => t.Type == TransactionType.Income ? t.Amount : -t.Amount);

    public static IEnumerable<Transaction> Filter(
        IEnumerable<Transaction> source,
        DateTime? from,
        DateTime? to,
        int? categoryId,
        string? search)
    {
        if (from.HasValue)
            source = source.Where(t => t.Date >= from.Value);
        if (to.HasValue)
            source = source.Where(t => t.Date <= to.Value);
        if (categoryId.HasValue)
            source = source.Where(t => t.CategoryId == categoryId.Value);
        if (!string.IsNullOrWhiteSpace(search))
            source = source.Where(t => t.Comment.Contains(search, StringComparison.OrdinalIgnoreCase));
        return source;
    }

    public static BudgetStatusCode GetBudgetStatus(decimal spent, decimal limit)
    {
        if (limit <= 0) return BudgetStatusCode.Normal;
        if (spent >= limit) return BudgetStatusCode.Over;
        if (spent >= limit * 0.8m) return BudgetStatusCode.Warning;
        return BudgetStatusCode.Normal;
    }
}
