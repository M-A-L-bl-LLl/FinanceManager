using CommunityToolkit.Mvvm.ComponentModel;

namespace FinanceManager.UI.ViewModels;

public abstract class BaseViewModel : ObservableObject
{
    private bool _isLoading;

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public virtual Task LoadAsync() => Task.CompletedTask;
}
