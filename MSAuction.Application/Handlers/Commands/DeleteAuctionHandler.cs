using MassTransit;
using MediatR;
using MongoDB.Driver;
using MSAuction.Application.Commands;
using MSAuction.Interfaces.Persistence.Repositories.PostgreSQL;
using MSAuction.Commons.DTOs;
using MSAuction.Infrastructure.EventBus.Events;

namespace MSAuction.Application.Handlers.Commands;

public class DeleteAuctionHandler : IRequestHandler<DeleteAuctionCommand, Unit>
{
    private readonly IAuctionRepository _repository;
    private readonly IMongoCollection<MongoAuction> _mongoCollection;
    private readonly IPublishEndpoint _publishEndpoint;

    public DeleteAuctionHandler(
        IAuctionRepository repository,
        IMongoClient mongoClient,
        IPublishEndpoint publishEndpoint)
    {
        _repository = repository;
        _mongoCollection = mongoClient
            .GetDatabase("Subastas2025")
            .GetCollection<MongoAuction>("Auctions");

        _publishEndpoint = publishEndpoint;
    }

    public async Task<Unit> Handle(DeleteAuctionCommand request, CancellationToken cancellationToken)
    {
        // Verifica la existencia en Mongo
        var mongoAuction = await _mongoCollection
            .Find(a => a.AuctionId == request.AuctionId)
            .FirstOrDefaultAsync();

        if (mongoAuction is null || mongoAuction.Status != "pending")
            throw new InvalidOperationException("Solo se pueden eliminar subastas en estado pending.");

        if (mongoAuction.UserId != request.UserId)
            throw new UnauthorizedAccessException("No puedes eliminar subastas de otro usuario.");

        // Eliminar en PostgreSQL
        await _repository.DeleteAsync(request.AuctionId);

        // Publicar evento para eliminar en Mongo
        await _publishEndpoint.Publish(new AuctionDeletedEvent(request.AuctionId));
        Console.WriteLine("Publicado");

        return Unit.Value;
    }
}