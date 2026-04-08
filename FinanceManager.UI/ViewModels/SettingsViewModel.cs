using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanceManager.UI.Services;
using FinanceManager.UI.Views.Dialogs;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace FinanceManager.UI.ViewModels;

public partial class SettingsViewModel : BaseViewModel
{
    private static readonly string DbPath =
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "finance.db");

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

    [RelayCommand]
    private async Task ExportDatabase()
    {
        if (!File.Exists(DbPath))
        {
            await ShowAlert("Экспорт", "Файл базы данных не найден.", "AlertOutline", "#F44336");
            return;
        }

        var fileDialog = new SaveFileDialog
        {
            Title = "Экспорт базы данных",
            FileName = $"finance_backup_{DateTime.Now:yyyy-MM-dd}",
            Filter = "SQLite Database (*.db)|*.db",
            DefaultExt = ".db"
        };

        if (fileDialog.ShowDialog() != true) return;

        File.Copy(DbPath, fileDialog.FileName, overwrite: true);
        await ShowAlert("Экспорт", "База данных успешно экспортирована.", "CheckCircleOutline", "#4CAF50");
    }

    [RelayCommand]
    private async Task ImportDatabase()
    {
        var fileDialog = new OpenFileDialog
        {
            Title = "Импорт базы данных",
            Filter = "SQLite Database (*.db)|*.db",
            DefaultExt = ".db"
        };

        if (fileDialog.ShowDialog() != true) return;

        var confirmed = await ShowConfirm(
            "Импорт базы данных",
            "Все текущие данные будут заменены данными из выбранного файла. Приложение перезапустится автоматически.\n\nПродолжить?");

        if (confirmed is not "true") return;

        File.Copy(fileDialog.FileName, DbPath, overwrite: true);

        Process.Start(Process.GetCurrentProcess().MainModule!.FileName!);
        Application.Current.Shutdown();
    }

    private static Task<object?> ShowAlert(string title, string message, string icon = "InformationOutline", string iconColor = "#7C4DFF")
    {
        var view = new MessageDialogView
        {
            DataContext = new MessageDialogViewModel
            {
                Title = title,
                Message = message,
                Icon = icon,
                IconColor = iconColor,
                ConfirmText = "ОК"
            }
        };
        return DialogHost.Show(view, "RootDialog");
    }

    private static Task<object?> ShowConfirm(string title, string message)
    {
        var view = new MessageDialogView
        {
            DataContext = new MessageDialogViewModel
            {
                Title = title,
                Message = message,
                Icon = "HelpCircleOutline",
                IconColor = "#FF9800",
                ConfirmText = "ДА",
                CancelText = "ОТМЕНА"
            }
        };
        return DialogHost.Show(view, "RootDialog");
    }
}
