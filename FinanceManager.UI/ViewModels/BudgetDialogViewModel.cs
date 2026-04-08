using CommunityToolkit.Mvvm.ComponentModel;
using System.Globalization;

namespace FinanceManager.UI.ViewModels;

public partial class BudgetDialogViewModel : ObservableObject
{
    [ObservableProperty] private string limitText = "";

    public string CategoryName { get; init; } = "";
    public string CategoryIcon { get; init; } = "Wallet";
    public string CategoryColor { get; init; } = "#607D8B";

    public bool TryGetLimit(out decimal limit)
    {
        var normalized = LimitText.Replace(',', '.');
        return decimal.TryParse(normalized, NumberStyles.Any, CultureInfo.InvariantCulture, out limit)
               && limit > 0;
    }
}
