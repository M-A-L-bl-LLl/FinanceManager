using FinanceManager.Core.Services;

namespace FinanceManager.UI.ViewModels;

public class BudgetItemDto
{
    public int CategoryId { get; init; }
    public string CategoryName { get; init; } = "";
    public string CategoryIcon { get; init; } = "Wallet";
    public string CategoryColor { get; init; } = "#607D8B";

    public int? BudgetId { get; init; }
    public decimal Limit { get; init; }
    public decimal Spent { get; init; }

    public bool HasBudget => BudgetId.HasValue && Limit > 0;

    public double Progress => HasBudget ? Math.Min((double)(Spent / Limit), 1.5) : 0;
    public double ProgressPercent => Progress * 100;

    public BudgetStatusCode Status => FinanceCalculator.GetBudgetStatus(Spent, HasBudget ? Limit : 0);

    public string StatusColor => Status switch
    {
        BudgetStatusCode.Over => "#F44336",
        BudgetStatusCode.Warning => "#FF9800",
        _ => "#4CAF50"
    };

    public string SpentText => $"₽ {Spent:N2}";
    public string LimitText => HasBudget ? $"/ ₽ {Limit:N2}" : "Лимит не установлен";
    public string PercentText => HasBudget ? $"{(int)(Spent / Limit * 100)}%" : "";
}
