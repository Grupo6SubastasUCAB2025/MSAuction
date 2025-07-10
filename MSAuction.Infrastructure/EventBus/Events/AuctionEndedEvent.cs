namespace MSAuction.Infrastructure.EventBus.Events;

public class AuctionEndedEvent
{
    public int AuctionId { get; set; }

    public AuctionEndedEvent(int auctionId)
    {
        AuctionId = auctionId;
    }
}