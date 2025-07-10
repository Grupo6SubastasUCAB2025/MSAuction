namespace MSAuction.Infrastructure.EventBus.Events;

public class AuctionUpdatedEvent
{
    public int AuctionId { get; set; }
    public int ProductId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public decimal InitialPrice { get; set; }
    public decimal MinIncrement { get; set; }
    public decimal? ReservePrice { get; set; }
    public required string Status { get; set; }
    public required int UserId { get; set; }
}