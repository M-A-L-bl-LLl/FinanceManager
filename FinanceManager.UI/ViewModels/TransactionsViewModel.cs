using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanceManager.Core.Interfaces;
using FinanceManager.Core.Models;
using FinanceManager.UI.Services;
using FinanceManager.UI.Views.Dialogs;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using System.Collections.ObjectModel;

namespace FinanceManager.UI.ViewModels;

public partial class TransactionsViewModel : BaseViewModel
{
    private readonly ITransactionRepository _transactionRepo;
    private readonly ICategoryRepository _categoryRepo;

    public ObservableCollection<TransactionDto> Transactions { get; } = new();
    public ObservableCollection<Category> Categories { get; } = new();

    [ObservableProperty] private DateTime? filterFrom;
    [ObservableProperty] private DateTime? filterTo;
    [ObservableProperty] private Category? filterCategory;
    [ObservableProperty] private string searchText = "";

    public TransactionsViewModel(ITransactionRepository transactionRepo, ICategoryRepository categoryRepo)
    {
        _transactionRepo = transactionRepo;
        _categoryRepo = categoryRepo;
    }

    public override async Task LoadAsync()
    {
        IsLoading = true;
        try
        {
            await LoadCategoriesAsync();
            await RefreshListAsync();
        }
        finally { IsLoading = false; }
    }

    private async Task LoadCategoriesAsync()
    {
        var cats = await _categoryRepo.GetAllAsync();
        Categories.Clear();
        foreach (var c in cats) Categories.Add(c);
    }

    [RelayCommand]
    private async Task ApplyFilters()
    {
        IsLoading = true;
        try { await RefreshListAsync(); }
        finally { IsLoading = false; }
    }

    [RelayCommand]
    private async Task ClearFilters()
    {
        FilterFrom = null;
        FilterTo = null;
        FilterCategory = null;
        SearchText = "";
        await ApplyFilters();
    }

    [RelayCommand]
    private async Task AddTransaction()
    {
        var dialogVm = BuildDialogVm(null);
        var view = new TransactionDialogView { DataContext = dialogVm };
        var result = await DialogHost.Show(view, "RootDialog");

        if (result is true && dialogVm.TryGetAmount(out var amount) && dialogVm.SelectedCategory is not null)
        {
            await _transactionRepo.AddAsync(new Transaction
            {
                Amount = amount,
                Date = dialogVm.Date,
                Comment = dialogVm.Comment,
                Type = dialogVm.Type,
                CategoryId = dialogVm.SelectedCategory.Id
            });
            await RefreshListAsync();
        }
    }

    [RelayCommand]
    private async Task EditTransaction(TransactionDto dto)
    {
        var dialogVm = BuildDialogVm(dto);
        var view = new TransactionDialogView { DataContext = dialogVm };
        var result = await DialogHost.Show(view, "RootDialog");

        if (result is true && dialogVm.TryGetAmount(out var amount) && dialogVm.SelectedCategory is not null)
        {
            var entity = await _transactionRepo.GetByIdAsync(dto.Id);
            if (entity is null) return;

            entity.Amount = amount;
            entity.Date = dialogVm.Date;
            entity.Comment = dialogVm.Comment;
            entity.Type = dialogVm.Type;
            entity.CategoryId = dialogVm.SelectedCategory.Id;
            await _transactionRepo.UpdateAsync(entity);
            await RefreshListAsync();
        }
    }

    [RelayCommand]
    private async Task DeleteTransaction(TransactionDto dto)
    {
        await _transactionRepo.DeleteAsync(dto.Id);
        Transactions.Remove(dto);
    }

    [RelayCommand]
    private void ExportToExcel()
    {
        if (!Transactions.Any()) return;

        var dialog = new SaveFileDialog
        {
            Filter = "Excel файлы (*.xlsx)|*.xlsx",
            FileName = $"Транзакции_{DateTime.Today:yyyy-MM-dd}.xlsx"
        };

        if (dialog.ShowDialog() == true)
            ExcelExportService.Export(dialog.FileName, Transactions);
    }

    private TransactionDialogViewModel BuildDialogVm(TransactionDto? dto)
    {
        var vm = new TransactionDialogViewModel();
        foreach (var c in Categories) vm.Categories.Add(c);

        if (dto is not null)
        {
            vm.IsEditMode = true;
            vm.EditId = dto.Id;
            vm.AmountText = dto.Amount.ToString("F2");
            vm.Date = dto.Date;
            vm.Comment = dto.Comment;
            vm.IsExpense = dto.Type == TransactionType.Expense;
            vm.SelectedCategory = vm.Categories.FirstOrDefault(c => c.Id == dto.CategoryId);
        }

        return vm;
    }

    private async Task RefreshListAsync()
    {
        IEnumerable<Transaction> source;

        if (FilterFrom.HasValue && FilterTo.HasValue)
            source = await _transactionRepo.GetByPeriodAsync(FilterFrom.Value, FilterTo.Value.AddDays(1).AddTicks(-1));
        else
            source = await _transactionRepo.GetAllAsync();

        if (FilterCategory is not null)
            source = source.Where(t => t.CategoryId == FilterCategory.Id);

        if (!string.IsNullOrWhiteSpace(SearchText))
            source = source.Where(t => t.Comment.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

        Transactions.Clear();
        foreach (var t in source.OrderByDescending(t => t.Date))
        {
            Transactions.Add(new TransactionDto
            {
                Id = t.Id,
                Amount = t.Amount,
                Date = t.Date,
                Comment = t.Comment,
                Type = t.Type,
                CategoryId = t.CategoryId,
                CategoryName = t.Category?.Name ?? "",
                CategoryIcon = t.Category?.Icon ?? "Wallet",
                CategoryColor = t.Category?.Color ?? "#607D8B"
            });
        }
    }
}
