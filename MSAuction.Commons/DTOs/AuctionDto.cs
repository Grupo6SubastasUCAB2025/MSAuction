namespace MSAuction.Commons.DTOs;

public class AuctionDto
{
    public int ProductId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public decimal InitialPrice { get; set; }
    public decimal? ReservePrice { get; set; }
    public decimal MinIncrement { get; set; }
    public string? Conditions { get; set; }
    public string Type { get; set; } = "normal";
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}