namespace MSAuction.Interfaces.Persistence.Repositories.Mongo;
using MSAuction.Commons.DTOs;

public interface IMongoAuctionRepository
{
    Task AddAsync(MongoAuction auction);           // Crear subasta en Mongo
    Task UpdateAsync(MongoAuction auction);         // Actualizar subasta en Mongo
    Task DeleteAsync(int auctionId);                // Eliminar subasta de Mongo
    Task<MongoAuction> GetByIdAsync(int auctionId); // Obtener subasta por ID en Mongo
}