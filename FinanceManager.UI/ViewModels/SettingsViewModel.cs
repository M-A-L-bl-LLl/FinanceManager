using CommunityToolkit.Mvvm.ComponentModel;
using FinanceManager.UI.Services;
using MaterialDesignThemes.Wpf;

namespace FinanceManager.UI.ViewModels;

public partial class SettingsViewModel : BaseViewModel
{
    private bool _loading;

    [ObservableProperty] private bool isDarkTheme;

    public override Task LoadAsync()
    {
        _loading = true;
        IsDarkTheme = SettingsService.Load().IsDarkTheme;
        _loading = false;
        return Task.CompletedTask;
    }

    partial void OnIsDarkThemeChanged(bool value)
    {
        if (_loading) return;
        ApplyTheme(value);
        SettingsService.Save(new AppSettings { IsDarkTheme = value });
    }

    public static void ApplyTheme(bool isDark)
    {
        var helper = new PaletteHelper();
        var theme = helper.GetTheme();
        theme.SetBaseTheme(isDark ? BaseTheme.Dark : BaseTheme.Light);
        helper.SetTheme(theme);
    }
}
