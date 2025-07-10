using MediatR;
using MSAuction.Application.Commands;
using MSAuction.Interfaces.Persistence.Repositories.PostgreSQL;
using MSAuction.Infrastructure.EventBus.Events;

namespace MSAuction.Application.Handlers.Commands;

public class UpdateAuctionHandler : IRequestHandler<UpdateAuctionCommand, bool>
{
    private readonly IAuctionRepository _repository;

    public UpdateAuctionHandler(IAuctionRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(UpdateAuctionCommand request, CancellationToken cancellationToken)
    {
        var auction = await _repository.GetByIdAsync(request.AuctionId);

        if (auction == null || auction.UserId != request.UserId)
        {
            return false;  // La subasta no existe o no es el propietario
        }

        // Validar que no se esté modificando una subasta ya comenzada
        if (auction.Status == "active" || auction.Status == "finalizada")
        {
            return false;
        }

        // Actualizar los campos
        auction.ProductId = request.AuctionDto.ProductId;
        auction.Title = request.AuctionDto.Title;
        auction.Description = request.AuctionDto.Description;
        auction.InitialPrice = request.AuctionDto.InitialPrice;
        auction.MinIncrement = request.AuctionDto.MinIncrement;
        auction.ReservePrice = request.AuctionDto.ReservePrice;

        // Guardar los cambios
        await _repository.UpdateAsync(auction);

        var auctionUpdatedEvent = new AuctionUpdatedEvent
        {
            AuctionId = auction.Id,
            ProductId = auction.ProductId,
            Title = auction.Title,
            Description = auction.Description,
            InitialPrice = auction.InitialPrice,
            MinIncrement = auction.MinIncrement,
            ReservePrice = auction.ReservePrice,
            Status = auction.Status,
            UserId = auction.UserId
        };

        return true;
    }
}