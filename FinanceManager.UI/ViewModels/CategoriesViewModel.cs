using CommunityToolkit.Mvvm.Input;
using FinanceManager.Core.Interfaces;
using FinanceManager.Core.Models;
using FinanceManager.UI.Views.Dialogs;
using MaterialDesignThemes.Wpf;
using System.Collections.ObjectModel;

namespace FinanceManager.UI.ViewModels;

public partial class CategoriesViewModel : BaseViewModel
{
    private readonly ICategoryRepository _categoryRepo;

    public ObservableCollection<Category> IncomeCategories { get; } = new();
    public ObservableCollection<Category> ExpenseCategories { get; } = new();

    public CategoriesViewModel(ICategoryRepository categoryRepo)
    {
        _categoryRepo = categoryRepo;
    }

    public override async Task LoadAsync()
    {
        IsLoading = true;
        try { await RefreshAsync(); }
        finally { IsLoading = false; }
    }

    [RelayCommand]
    private async Task AddCategory()
    {
        var dialogVm = new CategoryDialogViewModel();
        var view = new CategoryDialogView { DataContext = dialogVm };
        var result = await DialogHost.Show(view, "RootDialog");

        if (result is true && !string.IsNullOrWhiteSpace(dialogVm.Name))
        {
            await _categoryRepo.AddAsync(new Category
            {
                Name = dialogVm.Name.Trim(),
                Icon = dialogVm.SelectedIcon,
                Color = dialogVm.SelectedColor,
                Type = dialogVm.Type
            });
            await RefreshAsync();
        }
    }

    [RelayCommand]
    private async Task EditCategory(Category category)
    {
        var dialogVm = new CategoryDialogViewModel
        {
            IsEditMode = true,
            EditId = category.Id,
            Name = category.Name,
            SelectedIcon = category.Icon,
            SelectedColor = category.Color,
            IsExpense = category.Type == TransactionType.Expense
        };

        var view = new CategoryDialogView { DataContext = dialogVm };
        var result = await DialogHost.Show(view, "RootDialog");

        if (result is true && !string.IsNullOrWhiteSpace(dialogVm.Name))
        {
            category.Name = dialogVm.Name.Trim();
            category.Icon = dialogVm.SelectedIcon;
            category.Color = dialogVm.SelectedColor;
            category.Type = dialogVm.Type;
            await _categoryRepo.UpdateAsync(category);
            await RefreshAsync();
        }
    }

    [RelayCommand]
    private async Task DeleteCategory(Category category)
    {
        await _categoryRepo.DeleteAsync(category.Id);
        IncomeCategories.Remove(category);
        ExpenseCategories.Remove(category);
    }

    private async Task RefreshAsync()
    {
        var all = (await _categoryRepo.GetAllAsync()).ToList();

        IncomeCategories.Clear();
        ExpenseCategories.Clear();

        foreach (var c in all)
        {
            if (c.Type == TransactionType.Income) IncomeCategories.Add(c);
            else ExpenseCategories.Add(c);
        }
    }
}
