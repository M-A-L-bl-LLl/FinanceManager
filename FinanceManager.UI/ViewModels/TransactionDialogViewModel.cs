using CommunityToolkit.Mvvm.ComponentModel;
using FinanceManager.Core.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;

namespace FinanceManager.UI.ViewModels;

public partial class TransactionDialogViewModel : ObservableObject, IDataErrorInfo
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsIncome))]
    private bool isExpense = true;

    public bool IsIncome
    {
        get => !IsExpense;
        set => IsExpense = !value;
    }

    [ObservableProperty] private string amountText = "";
    [ObservableProperty] private DateTime date = DateTime.Today;
    [ObservableProperty] private string comment = "";
    [ObservableProperty] private Category? selectedCategory;

    public ObservableCollection<Category> Categories { get; } = new();

    public bool IsEditMode { get; set; }
    public int EditId { get; set; }

    public TransactionType Type => IsExpense ? TransactionType.Expense : TransactionType.Income;

    public bool TryGetAmount(out decimal amount)
    {
        var normalized = AmountText.Replace(',', '.');
        return decimal.TryParse(normalized, NumberStyles.Any, CultureInfo.InvariantCulture, out amount)
               && amount > 0;
    }

    partial void OnIsExpenseChanged(bool value) => OnPropertyChanged(nameof(Type));

    public string Error => string.Empty;

    public string this[string columnName] => columnName switch
    {
        nameof(AmountText) => !TryGetAmount(out _) ? "Введите сумму больше 0" : string.Empty,
        nameof(Date) => Date > DateTime.Today ? "Дата не может быть в будущем" : string.Empty,
        nameof(SelectedCategory) => SelectedCategory is null ? "Выберите категорию" : string.Empty,
        _ => string.Empty
    };
}
