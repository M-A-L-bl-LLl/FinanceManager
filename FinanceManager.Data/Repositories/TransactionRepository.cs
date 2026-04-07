using FinanceManager.Core.Interfaces;
using FinanceManager.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceManager.Data.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly AppDbContext _context;

    public TransactionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Transaction>> GetAllAsync()
        => await _context.Transactions.Include(t => t.Category).OrderByDescending(t => t.Date).ToListAsync();

    public async Task<IEnumerable<Transaction>> GetByPeriodAsync(DateTime from, DateTime to)
        => await _context.Transactions
            .Include(t => t.Category)
            .Where(t => t.Date >= from && t.Date <= to)
            .OrderByDescending(t => t.Date)
            .ToListAsync();

    public async Task<Transaction?> GetByIdAsync(int id)
        => await _context.Transactions.Include(t => t.Category).FirstOrDefaultAsync(t => t.Id == id);

    public async Task AddAsync(Transaction transaction)
    {
        await _context.Transactions.AddAsync(transaction);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Transaction transaction)
    {
        _context.Transactions.Update(transaction);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var transaction = await _context.Transactions.FindAsync(id);
        if (transaction is not null)
        {
            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<decimal> GetBalanceAsync(DateTime from, DateTime to)
    {
        var transactions = await GetByPeriodAsync(from, to);
        return transactions.Sum(t => t.Type == TransactionType.Income ? t.Amount : -t.Amount);
    }
}
