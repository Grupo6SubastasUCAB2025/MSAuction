namespace MSAuction.Infrastructure.EventBus.Events;

public class AuctionDeletedEvent
{
    public int AuctionId { get; set; }

    public AuctionDeletedEvent(int auctionId)
    {
        AuctionId = auctionId;
    }
}