namespace FinanceManager.Core.Interfaces;

public interface INavigationService
{
    void NavigateTo<TViewModel>() where TViewModel : class;
}
