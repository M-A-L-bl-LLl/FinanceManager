using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanceManager.UI.Services;

namespace FinanceManager.UI.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly NavigationService _navigation;

    [ObservableProperty]
    private string _currentPageTitle = "Дашборд";

    public ObservableObject? CurrentViewModel => _navigation.CurrentViewModel;

    public MainViewModel(NavigationService navigation)
    {
        _navigation = navigation;
        _navigation.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(NavigationService.CurrentViewModel))
                OnPropertyChanged(nameof(CurrentViewModel));
        };
    }

    [RelayCommand]
    private void NavigateToDashboard()
    {
        _navigation.NavigateTo<DashboardViewModel>();
        CurrentPageTitle = "Дашборд";
    }

    [RelayCommand]
    private void NavigateToTransactions()
    {
        _navigation.NavigateTo<TransactionsViewModel>();
        CurrentPageTitle = "Транзакции";
    }

    [RelayCommand]
    private void NavigateToCategories()
    {
        _navigation.NavigateTo<CategoriesViewModel>();
        CurrentPageTitle = "Категории";
    }

    [RelayCommand]
    private void NavigateToBudgets()
    {
        _navigation.NavigateTo<BudgetsViewModel>();
        CurrentPageTitle = "Бюджеты";
    }

    [RelayCommand]
    private void NavigateToSettings()
    {
        _navigation.NavigateTo<SettingsViewModel>();
        CurrentPageTitle = "Настройки";
    }

    public void Initialize() => NavigateToDashboard();
}
