namespace MSAuction.Infrastructure.EventBus.Events;

public class AuctionActivedEvent
{
    public int AuctionId { get; set; }

    public AuctionActivedEvent(int auctionId)
    {
        AuctionId = auctionId;
    }
}