using FinanceManager.UI.ViewModels;
using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Windows.Controls;

namespace FinanceManager.UI.Views.Dialogs;

public partial class TransactionDialogView : UserControl
{
    public TransactionDialogView()
    {
        InitializeComponent();
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        var vm = (TransactionDialogViewModel)DataContext;

        if (!vm.TryGetAmount(out _))
        {
            MessageBox.Show("Введите корректную сумму (больше 0).", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (vm.SelectedCategory is null)
        {
            MessageBox.Show("Выберите категорию.", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        DialogHost.CloseDialogCommand.Execute(true, this);
    }
}
