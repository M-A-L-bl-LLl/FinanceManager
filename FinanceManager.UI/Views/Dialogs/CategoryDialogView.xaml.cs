using FinanceManager.UI.ViewModels;
using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Windows.Controls;

namespace FinanceManager.UI.Views.Dialogs;

public partial class CategoryDialogView : UserControl
{
    public CategoryDialogView()
    {
        InitializeComponent();
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        var vm = (CategoryDialogViewModel)DataContext;

        if (string.IsNullOrWhiteSpace(vm.Name))
        {
            MessageBox.Show("Введите название категории.", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        DialogHost.CloseDialogCommand.Execute(true, this);
    }
}
