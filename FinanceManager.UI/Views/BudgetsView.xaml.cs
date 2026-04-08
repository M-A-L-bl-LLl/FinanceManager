using FinanceManager.UI.ViewModels;
using System.Windows.Controls;

namespace FinanceManager.UI.Views;

public partial class BudgetsView : UserControl
{
    public BudgetsView() => InitializeComponent();

    private void MonthCalendar_DisplayModeChanged(object sender, CalendarModeChangedEventArgs e)
    {
        if (sender is not Calendar cal || DataContext is not BudgetsViewModel vm) return;

        if (e.OldMode == CalendarMode.Year && e.NewMode == CalendarMode.Month)
        {
            _ = vm.GoToMonthAsync(cal.DisplayDate);
            cal.DisplayMode = CalendarMode.Year;
        }
    }
}
