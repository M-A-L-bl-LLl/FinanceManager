using FinanceManager.Core.Interfaces;
using FinanceManager.Data;
using FinanceManager.Data.Repositories;
using FinanceManager.UI.Services;
using FinanceManager.UI.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Threading;

namespace FinanceManager.UI;

public partial class App : Application
{
    private readonly IServiceProvider _serviceProvider;

    public App()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // Database
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite("Data Source=finance.db"));

        // Repositories
        services.AddTransient<ITransactionRepository, TransactionRepository>();
        services.AddTransient<ICategoryRepository, CategoryRepository>();
        services.AddTransient<IBudgetRepository, BudgetRepository>();

        // Navigation
        services.AddSingleton<NavigationService>();

        // ViewModels
        services.AddSingleton<MainViewModel>();
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<TransactionsViewModel>();
        services.AddTransient<CategoriesViewModel>();
        services.AddTransient<BudgetsViewModel>();
        services.AddTransient<SettingsViewModel>();

        // Views
        services.AddSingleton<MainWindow>();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        SettingsViewModel.ApplyTheme(SettingsService.Load().IsDarkTheme);

        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();

        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.DataContext = _serviceProvider.GetRequiredService<MainViewModel>();
        mainWindow.Show();

        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.Initialize();
    }
}
