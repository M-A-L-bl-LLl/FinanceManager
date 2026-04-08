using FinanceManager.Core.Models;

namespace FinanceManager.UI.ViewModels;

public class TransactionDto
{
    public int Id { get; init; }
    public decimal Amount { get; init; }
    public DateTime Date { get; init; }
    public string Comment { get; init; } = "";
    public TransactionType Type { get; init; }
    public int CategoryId { get; init; }
    public string CategoryName { get; init; } = "";
    public string CategoryIcon { get; init; } = "Wallet";
    public string CategoryColor { get; init; } = "#607D8B";

    public string FormattedAmount => Type == TransactionType.Income
        ? $"+ ₽ {Amount:N2}"
        : $"- ₽ {Amount:N2}";

    public string AmountColor => Type == TransactionType.Income ? "#4CAF50" : "#F44336";
    public string TypeLabel => Type == TransactionType.Income ? "Доход" : "Расход";
    public string TypeBadgeBackground => Type == TransactionType.Income ? "#E8F5E9" : "#FFEBEE";
}
