using FinanceManager.UI.ViewModels;
using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Windows.Controls;

namespace FinanceManager.UI.Views.Dialogs;

public partial class BudgetDialogView : UserControl
{
    public BudgetDialogView()
    {
        InitializeComponent();
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        var vm = (BudgetDialogViewModel)DataContext;

        if (!vm.TryGetLimit(out _))
        {
            MessageBox.Show("Введите корректный лимит (больше 0).", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        DialogHost.CloseDialogCommand.Execute(true, this);
    }
}
