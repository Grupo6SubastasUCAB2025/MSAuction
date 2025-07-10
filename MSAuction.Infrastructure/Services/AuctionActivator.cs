using MassTransit;
using MSAuction.Interfaces.Persistence.Repositories.PostgreSQL;
using MSAuction.Interfaces.Services;
using MSAuction.Infrastructure.EventBus.Events;

namespace MSAuction.Infrastructure.Services;

public class AuctionActivator : IAuctionActivator
{
    private readonly IAuctionRepository _repository;
    private readonly IPublishEndpoint _publishEndpoint;

    public AuctionActivator(IAuctionRepository repository, IPublishEndpoint publishEndpoint)
    {
        _repository = repository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task ActivateAuctionAsync(int auctionId)
    {
        var auction = await _repository.GetByIdAsync(auctionId);
        if (auction is null || auction.Status != "pending") return;

        auction.MarkAsActive(); // Método de dominio que cambia el estado
        await _repository.UpdateAsync(auction);

        await _publishEndpoint.Publish(new AuctionActivedEvent(auctionId));
    }
}