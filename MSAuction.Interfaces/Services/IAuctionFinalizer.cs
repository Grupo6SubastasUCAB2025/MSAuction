namespace MSAuction.Interfaces.Services;

public interface IAuctionFinalizer
{
    Task FinalizeAuctionAsync(int auctionId);
}