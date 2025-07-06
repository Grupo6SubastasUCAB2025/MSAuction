namespace MSAuction.Infrastructure.EventBus.Events;

public class AuctionUpdatedEvent
{
    public int AuctionId { get; set; }
    public string? Title { get; set; }
    public decimal InitialPrice { get; set; }
    public DateTime EndDate { get; set; }
    public string? Status { get; set; }
}