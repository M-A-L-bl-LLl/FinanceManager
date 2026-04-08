using CommunityToolkit.Mvvm.ComponentModel;
using FinanceManager.Core.Models;
using System.ComponentModel;

namespace FinanceManager.UI.ViewModels;

public partial class CategoryDialogViewModel : ObservableObject, IDataErrorInfo
{
    [ObservableProperty] private string name = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsIncome))]
    private bool isExpense = true;

    public bool IsIncome
    {
        get => !IsExpense;
        set => IsExpense = !value;
    }

    [ObservableProperty] private string selectedIcon = "Wallet";
    [ObservableProperty] private string selectedColor = "#607D8B";

    public bool IsEditMode { get; set; }
    public int EditId { get; set; }

    public TransactionType Type => IsExpense ? TransactionType.Expense : TransactionType.Income;

    public IReadOnlyList<string> AvailableIcons { get; } = CategoryConstants.Icons;
    public IReadOnlyList<string> AvailableColors { get; } = CategoryConstants.Colors;

    partial void OnIsExpenseChanged(bool value) => OnPropertyChanged(nameof(Type));

    public string Error => string.Empty;

    public string this[string columnName] => columnName switch
    {
        nameof(Name) => string.IsNullOrWhiteSpace(Name) ? "Введите название категории" : string.Empty,
        _ => string.Empty
    };
}
