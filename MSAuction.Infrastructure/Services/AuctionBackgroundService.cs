using MSAuction.Interfaces.Persistence.Repositories.PostgreSQL;

namespace MSAuction.Infrastructure.Services;

public class AuctionBackgroundService
{
    private readonly IAuctionRepository _repository;

    public AuctionBackgroundService(IAuctionRepository repository)
    {
        _repository = repository;
    }

    public async Task FinalizeAuction(int auctionId)
    {
        var auction = await _repository.GetByIdAsync(auctionId);
        if (auction == null || auction.Status == "finalized")
            return;

        if (auction.EndDate <= DateTime.UtcNow)
        {
            auction.MarkAsEnded();
            await _repository.UpdateAsync(auction);
        }
    }
    
    public async Task ActivateAuction(int auctionId)
    {
        var auction = await _repository.GetByIdAsync(auctionId);
        if (auction == null || auction.Status == "active")
            return;

        if (auction.StartDate <= DateTime.UtcNow)
        {
            auction.MarkAsActive();
            await _repository.UpdateAsync(auction);
        }
    }
}