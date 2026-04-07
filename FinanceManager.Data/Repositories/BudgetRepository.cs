using FinanceManager.Core.Interfaces;
using FinanceManager.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceManager.Data.Repositories;

public class BudgetRepository : IBudgetRepository
{
    private readonly AppDbContext _context;

    public BudgetRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Budget>> GetByMonthAsync(int month, int year)
        => await _context.Budgets.Include(b => b.Category).Where(b => b.Month == month && b.Year == year).ToListAsync();

    public async Task<Budget?> GetByIdAsync(int id)
        => await _context.Budgets.Include(b => b.Category).FirstOrDefaultAsync(b => b.Id == id);

    public async Task<Budget?> GetByCategoryAndMonthAsync(int categoryId, int month, int year)
        => await _context.Budgets.FirstOrDefaultAsync(b => b.CategoryId == categoryId && b.Month == month && b.Year == year);

    public async Task AddAsync(Budget budget)
    {
        await _context.Budgets.AddAsync(budget);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Budget budget)
    {
        _context.Budgets.Update(budget);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var budget = await _context.Budgets.FindAsync(id);
        if (budget is not null)
        {
            _context.Budgets.Remove(budget);
            await _context.SaveChangesAsync();
        }
    }
}
