﻿using Microsoft.EntityFrameworkCore;
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

    public async Task<List<Purchase>> All() =>
        await _context
            .Purchases.Include(purchase => purchase.Sea)
            .Include(purchase => purchase.Round)
            .Include(purchase => purchase.Team)
            .ThenInclude(team => team.StartingSea)
            .ToListAsync();

    public async Task<List<Purchase>> TeamPurchasesAsync(Round round, Team team) =>
        (await All())
            .Where(purchase => purchase.Round.Id == round.Id && purchase.Team.Id == team.Id)
            .ToList();
}
