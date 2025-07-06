namespace MSAuction.Domain.Entities;

public class Auction
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int UserId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public decimal InitialPrice { get; set; }
    public decimal MinIncrement { get; set; }
    public decimal? ReservePrice { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = "pending";
    public string? Conditions { get; set; }
    public string Type { get; set; } = "normal";

    public void MarkAsEnded()
    {
        Status = "finalized";
    }

    public void MarkAsActive()
    {
        Status = "active";
    }
}