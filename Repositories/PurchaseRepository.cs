using Microsoft.EntityFrameworkCore;
using PirateConquest.Database;
using PirateConquest.Models;
using SQLitePCL;

namespace PirateConquest.Repositories;

public class PurchaseRepository
{
    private readonly AppDbContext _context;

    public PurchaseRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Purchase>> AllAsync() =>
        await _context
            .Purchases.Include(purchase => purchase.Sea)
            .Include(purchase => purchase.Round)
            .Include(purchase => purchase.Team)
            .ThenInclude(team => team.StartingSea)
            .OrderBy(purchase => purchase.Sea.Name)
            .ThenBy(purchase => purchase.Team.Name)
            .ThenBy(purchase => purchase.Points)
            .ToListAsync();

    public async Task AddAsync(Purchase purchase)
    {
        await _context.Purchases.AddAsync(purchase);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Purchase>> TeamPurchasesAsync(Round round, Team team) =>
        (await AllAsync())
            .Where(purchase => purchase.Round.Id == round.Id && purchase.Team.Id == team.Id)
            .ToList();

    public async Task<List<Purchase>> FromRoundAsync(Round round) =>
        (await AllAsync()).Where(purchase => purchase.Round.Id == round.Id).ToList();
}
