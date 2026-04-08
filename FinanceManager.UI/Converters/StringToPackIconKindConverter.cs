using MaterialDesignThemes.Wpf;
using System.Globalization;
using System.Windows.Data;

namespace FinanceManager.UI.Converters;

public class StringToPackIconKindConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string name && Enum.TryParse<PackIconKind>(name, out var kind))
            return kind;
        return PackIconKind.Wallet;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
