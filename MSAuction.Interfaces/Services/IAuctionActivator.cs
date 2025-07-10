namespace MSAuction.Interfaces.Services;

public interface IAuctionActivator
{
    Task ActivateAuctionAsync(int auctionId);
}