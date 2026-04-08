using FinanceManager.Core.Models;
using System.Collections.ObjectModel;

namespace FinanceManager.UI.ViewModels.Design;

public class TransactionsViewModelDesign
{
    public ObservableCollection<TransactionDto> Transactions { get; } = new()
    {
        new TransactionDto
        {
            Id = 1, Amount = 5000, Type = TransactionType.Income,
            Date = new DateTime(2026, 4, 1),
            CategoryName = "Зарплата", CategoryColor = "#7C4DFF", CategoryIcon = "Wallet",
            Comment = "Апрельская зарплата"
        },
        new TransactionDto
        {
            Id = 2, Amount = 1200, Type = TransactionType.Expense,
            Date = new DateTime(2026, 4, 3),
            CategoryName = "Продукты", CategoryColor = "#FF9800", CategoryIcon = "Cart",
            Comment = "Супермаркет"
        },
        new TransactionDto
        {
            Id = 3, Amount = 800, Type = TransactionType.Expense,
            Date = new DateTime(2026, 4, 5),
            CategoryName = "Транспорт", CategoryColor = "#2196F3", CategoryIcon = "Car",
            Comment = ""
        },
        new TransactionDto
        {
            Id = 4, Amount = 3500, Type = TransactionType.Income,
            Date = new DateTime(2026, 4, 7),
            CategoryName = "Фриланс", CategoryColor = "#9C27B0", CategoryIcon = "Laptop",
            Comment = "Проект UI"
        }
    };
}
