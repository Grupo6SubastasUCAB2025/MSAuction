using MediatR;
using MSAuction.Commons.DTOs;

namespace MSAuction.Application.Commands;

public class UpdateAuctionCommand : IRequest<bool>
{
    public int AuctionId { get; set; }
    public AuctionDto AuctionDto { get; set; }
    public int UserId { get; set; }

    public UpdateAuctionCommand(int auctionId, AuctionDto auctionDto, int userId)
    {
        AuctionId = auctionId;
        AuctionDto = auctionDto;
        UserId = userId;
    }
}