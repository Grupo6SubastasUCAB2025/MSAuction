using MSAuction.Infrastructure.Contexts;
using MSAuction.Interfaces.Persistence.Repositories.PostgreSQL;
using MSAuction.Domain.Entities;
using  Microsoft.EntityFrameworkCore;

namespace MSAuction.Infrastructure.Persistence.Repositories.PostgreSQL;


public class AuctionRepository : IAuctionRepository
{
    private readonly AppDbContext _context;

    public AuctionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> AddAsync(Auction auction)
    {
        await _context.Auctions.AddAsync(auction);
        await _context.SaveChangesAsync();
        return auction.Id; // ID generado por PostgreSQL (serial)
    }
    
    public async Task<Auction?> GetByIdAsync(int id)
    {
        // Obtén una subasta por su ID
        return await _context.Auctions
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task UpdateAsync(Auction auction)
    {
        // Actualiza la subasta en la base de datos
        _context.Auctions.Update(auction);
        await _context.SaveChangesAsync();
    }
}