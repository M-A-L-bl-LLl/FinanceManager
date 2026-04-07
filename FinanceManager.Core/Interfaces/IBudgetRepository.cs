using FinanceManager.Core.Models;

namespace FinanceManager.Core.Interfaces;

public interface IBudgetRepository
{
    Task<IEnumerable<Budget>> GetByMonthAsync(int month, int year);
    Task<Budget?> GetByIdAsync(int id);
    Task<Budget?> GetByCategoryAndMonthAsync(int categoryId, int month, int year);
    Task AddAsync(Budget budget);
    Task UpdateAsync(Budget budget);
    Task DeleteAsync(int id);
}
