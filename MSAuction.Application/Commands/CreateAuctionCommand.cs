namespace MSAuction.Application.Commands;
using MediatR;
using MSAuction.Commons.DTOs;

public class CreateAuctionCommand : IRequest<int> // Devuelve el ID int (serial en PostgreSQL)
{
    public AuctionDto Auction { get; set; }
    public int UserId { get; set; } // Usuario creador

    public CreateAuctionCommand(AuctionDto auction, int userId)
    {
        Auction = auction;
        UserId = userId;
    }
}