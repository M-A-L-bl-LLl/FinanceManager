using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanceManager.Core.Interfaces;
using FinanceManager.Core.Models;
using FinanceManager.UI.Views.Dialogs;
using MaterialDesignThemes.Wpf;
using System.Collections.ObjectModel;

namespace FinanceManager.UI.ViewModels;

public partial class BudgetsViewModel : BaseViewModel
{
    private readonly IBudgetRepository _budgetRepo;
    private readonly ICategoryRepository _categoryRepo;
    private readonly ITransactionRepository _transactionRepo;

    private static readonly string[] MonthNames =
    {
        "Январь", "Февраль", "Март", "Апрель", "Май", "Июнь",
        "Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь"
    };

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(MonthDisplay))]
    private int selectedMonth = DateTime.Today.Month;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(MonthDisplay))]
    private int selectedYear = DateTime.Today.Year;

    public string MonthDisplay => $"{MonthNames[SelectedMonth - 1]} {SelectedYear}";

    [ObservableProperty] private bool isCalendarOpen;

public ObservableCollection<BudgetItemDto> BudgetItems { get; } = new();

    public BudgetsViewModel(IBudgetRepository budgetRepo, ICategoryRepository categoryRepo,
        ITransactionRepository transactionRepo)
    {
        _budgetRepo = budgetRepo;
        _categoryRepo = categoryRepo;
        _transactionRepo = transactionRepo;
    }

    public override async Task LoadAsync()
    {
        IsLoading = true;
        try { await RefreshAsync(); }
        finally { IsLoading = false; }
    }

    public async Task GoToMonthAsync(DateTime date)
    {
        SelectedMonth = date.Month;
        SelectedYear = date.Year;
        IsCalendarOpen = false;
        await RefreshAsync();
    }

    [RelayCommand]
    private async Task PreviousMonth()
    {
        if (SelectedMonth == 1) { SelectedMonth = 12; SelectedYear--; }
        else SelectedMonth--;
        await RefreshAsync();
    }

    [RelayCommand]
    private async Task NextMonth()
    {
        if (SelectedMonth == 12) { SelectedMonth = 1; SelectedYear++; }
        else SelectedMonth++;
        await RefreshAsync();
    }

    [RelayCommand]
    private async Task SetBudget(BudgetItemDto item)
    {
        var dialogVm = new BudgetDialogViewModel
        {
            CategoryName = item.CategoryName,
            CategoryIcon = item.CategoryIcon,
            CategoryColor = item.CategoryColor,
            LimitText = item.HasBudget ? item.Limit.ToString("F2") : ""
        };

        var view = new BudgetDialogView { DataContext = dialogVm };
        var result = await DialogHost.Show(view, "RootDialog");

        if (result is true && dialogVm.TryGetLimit(out var limit))
        {
            var existing = await _budgetRepo.GetByCategoryAndMonthAsync(
                item.CategoryId, SelectedMonth, SelectedYear);

            if (existing is null)
            {
                await _budgetRepo.AddAsync(new Budget
                {
                    CategoryId = item.CategoryId,
                    Limit = limit,
                    Month = SelectedMonth,
                    Year = SelectedYear
                });
            }
            else
            {
                existing.Limit = limit;
                await _budgetRepo.UpdateAsync(existing);
            }

            await RefreshAsync();
        }
    }

    [RelayCommand]
    private async Task DeleteBudget(BudgetItemDto item)
    {
        if (item.BudgetId.HasValue)
        {
            await _budgetRepo.DeleteAsync(item.BudgetId.Value);
            await RefreshAsync();
        }
    }

    private async Task RefreshAsync()
    {
        var from = new DateTime(SelectedYear, SelectedMonth, 1);
        var to = from.AddMonths(1).AddTicks(-1);

        var categories = (await _categoryRepo.GetAllAsync())
            .Where(c => c.Type == TransactionType.Expense)
            .ToList();

        var budgets = (await _budgetRepo.GetByMonthAsync(SelectedMonth, SelectedYear))
            .ToDictionary(b => b.CategoryId);

        var transactions = (await _transactionRepo.GetByPeriodAsync(from, to))
            .Where(t => t.Type == TransactionType.Expense)
            .GroupBy(t => t.CategoryId)
            .ToDictionary(g => g.Key, g => g.Sum(t => t.Amount));

        BudgetItems.Clear();
        foreach (var cat in categories)
        {
            budgets.TryGetValue(cat.Id, out var budget);
            transactions.TryGetValue(cat.Id, out var spent);

            BudgetItems.Add(new BudgetItemDto
            {
                CategoryId = cat.Id,
                CategoryName = cat.Name,
                CategoryIcon = cat.Icon,
                CategoryColor = cat.Color,
                BudgetId = budget?.Id,
                Limit = budget?.Limit ?? 0,
                Spent = spent
            });
        }
    }
}
