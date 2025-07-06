using MediatR;

namespace MSAuction.Application.Commands;

public class DeleteAuctionCommand : IRequest<Unit>
{
    public int AuctionId { get; }
    public int UserId { get; }

    public DeleteAuctionCommand(int auctionId, int userId)
    {
        AuctionId = auctionId;
        UserId = userId;
    }
}