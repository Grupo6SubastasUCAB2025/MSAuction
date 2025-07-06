using MSAuction.Infrastructure.Contexts;

namespace MSAuction.Infrastructure.Persistence.Repositories.PostgreSQL;
using MSAuction.Interfaces.Persistence.Repositories.PostgreSQL;
using MSAuction.Domain.Entities;

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
}