using FinanceManager.Core.Models;

namespace FinanceManager.Core.Interfaces;

public interface ITransactionRepository
{
    Task<IEnumerable<Transaction>> GetAllAsync();
    Task<IEnumerable<Transaction>> GetByPeriodAsync(DateTime from, DateTime to);
    Task<Transaction?> GetByIdAsync(int id);
    Task AddAsync(Transaction transaction);
    Task UpdateAsync(Transaction transaction);
    Task DeleteAsync(int id);
    Task<decimal> GetBalanceAsync(DateTime from, DateTime to);
}
